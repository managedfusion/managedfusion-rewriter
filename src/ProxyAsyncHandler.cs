/** 
 * Copyright (C) 2007-2010 Nicholas Berardi, Managed Fusion, LLC (nick@managedfusion.com)
 * 
 * <author>Nicholas Berardi</author>
 * <author_email>nick@managedfusion.com</author_email>
 * <company>Managed Fusion, LLC</company>
 * <product>Url Rewriter and Reverse Proxy</product>
 * <license>Microsoft Public License (Ms-PL)</license>
 * <agreement>
 * This software, as defined above in <product />, is copyrighted by the <author /> and the <company />, all defined above.
 * 
 * For all binary distributions the <product /> is licensed for use under <license />.
 * For all source distributions please contact the <author /> at <author_email /> for a commercial license.
 * 
 * This copyright notice may not be removed and if this <product /> or any parts of it are used any other
 * packaged software, attribution needs to be given to the author, <author />.  This can be in the form of a textual
 * message at program startup or in documentation (online or textual) provided with the packaged software.
 * </agreement>
 * <product_url>http://www.managedfusion.com/products/url-rewriter/</product_url>
 * <license_url>http://www.managedfusion.com/products/url-rewriter/license.aspx</license_url>
 */

using System;
using System.Diagnostics;
using System.Linq;
using System.Web;
#if NET45
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
#endif

namespace ManagedFusion.Rewriter
{
	public class ProxyAsyncHandler : 
#if NET45
		HttpTaskAsyncHandler, IHttpProxyHandler
#else
		ProxyHandler, IHttpAsyncHandler
#endif
	{
#if !NET45
		[NonSerialized, DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<HttpContext> _proxyDelegate;
#endif

		/// <summary>
		/// Initializes a new instance of the <see cref="ProxyHandler"/> class.
		/// </summary>
		public ProxyAsyncHandler()
		{
		}

#if NET45
		#region IHttpProxyHandler Members

		/// <summary>
		/// Inits the specified request for the proxy.
		/// </summary>
		/// <param name="requestUrl">The request URL.</param>
		/// <param name="responseUrl">The response URL.</param>
		public void Init(Uri requestUrl, Uri responseUrl)
		{
			RequestUrl = requestUrl;
			ResponseUrl = responseUrl;
		}

		/// <summary>
		/// Gets or sets the requested URL.
		/// </summary>
		/// <value>The requested URL.</value>
		public Uri RequestUrl { get; private set; }

		/// <summary>
		/// Gets or sets the response URL.
		/// </summary>
		/// <value>The response URL.</value>
		public Uri ResponseUrl { get; private set; }

		#endregion

		public async override Task ProcessRequestAsync(HttpContext context)
		{
			Manager.Log("**********************************************************************************");

			var client = new HttpClient();
			var request = await GetRequestFromClient(context);

			// send the request to the target
			Manager.Log("Request: " + RequestUrl, "Proxy");
			var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

			if (response == null) {
				Manager.Log("No response was received, returning a '400 Bad Request' to the client.", "Proxy");
				throw new HttpException((int)HttpStatusCode.BadRequest, String.Format("The requested url, <{0}>, could not be found.", RequestUrl));
			}

			Manager.Log(String.Format("Received '{0}'", ((int)response.StatusCode)), "Proxy");

			// send the response to the client
			Manager.Log("Response: " + ResponseUrl, "Proxy");
			await SendResponseToClient(context, response);

			Manager.Log("**********************************************************************************");
		}

		/// <summary>
		/// Sends the request to server.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		private async Task<HttpRequestMessage> GetRequestFromClient(HttpContext context)
		{
			var request = new HttpRequestMessage(new HttpMethod(context.Request.HttpMethod), RequestUrl);
			var content = new StreamContent(context.Request.InputStream, Manager.Configuration.Rewriter.Proxy.BufferSize);

			var knownVerb = KnownHttpVerb.Parse(request.Method.Method);
			
			foreach (var name in context.Request.Headers.AllKeys) {
				HttpHeaders headers = request.Headers;

				if (name.StartsWith("content", StringComparison.OrdinalIgnoreCase) ||
					String.Equals("Allow", name, StringComparison.OrdinalIgnoreCase) ||
					String.Equals("Expires", name, StringComparison.OrdinalIgnoreCase) ||
					String.Equals("Last-Modified", name, StringComparison.OrdinalIgnoreCase))
					headers = content.Headers;

				headers.Add(name, context.Request.Headers.GetValues(name));
			}

			// add the vanity url to the header
			if (Manager.Configuration.Rewriter.AllowVanityHeader) {
				request.Headers.Add("X-Reverse-Proxied-By", Manager.RewriterUserAgent);
				request.Headers.Add("X-ManagedFusion-Rewriter-Version", Manager.RewriterVersion.ToString(2));
			}

			/*
			 * Add Proxy Standard Protocol Headers
			 */

			// http://en.wikipedia.org/wiki/X-Forwarded-For
			request.Headers.Add("X-Forwarded-For", context.Request.UserHostAddress);

			// http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.45
			string currentServerName = context.Request.ServerVariables["SERVER_NAME"];
			string currentServerPort = context.Request.ServerVariables["SERVER_PORT"];
			string currentServerProtocol = context.Request.ServerVariables["SERVER_PROTOCOL"];

			if (currentServerProtocol.IndexOf("/") >= 0)
				currentServerProtocol = currentServerProtocol.Substring(currentServerProtocol.IndexOf("/") + 1);

			string currentVia = String.Format("{0} {1}:{2} ({3})", currentServerProtocol, currentServerName, currentServerPort, Manager.RewriterNameAndVersion);

			request.Headers.Add("Via", currentVia);

			/*
			 * End - Add Proxy Standard Protocol Headers
			 */

			await OnRequestToTarget(context, request);

			// ContentLength is set to -1 if their is no data to send
			if (!knownVerb.ContentBodyNotAllowed) {
				request.Content = content;
			}

			return request;
		}

		/// <summary>
		/// Sends the response to client.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="response">The response.</param>
		private async Task SendResponseToClient(HttpContext context, HttpResponseMessage response)
		{
			context.Response.ClearHeaders();
			context.Response.ClearContent();

			/* You cannot set any cache headers through the API, because it will prevent the use of the cache headers
			 * added through the AppendHeader method.
			 */

			// add all the headers from the other proxied session to this request
			foreach(var header in response.Headers) {
				var name = header.Key;

				// don't add any of these headers
				// don't check for restricted response headers because HttpContext doesn't seem to care
				if (name == "Server" ||
					name == "X-Powered-By" ||
					name == "Date" ||
					name == "Host")
					continue;

				string[] values = header.Value.ToArray();
				if (values.Length == 0)
					continue;

				if (name == "Location") {
					try {
						string location = values[0];

						// make sure location is not empty before creating the URL
						if (!String.IsNullOrEmpty(location)) {
							// reminder: If location is an absolute URL, the Uri instance is created using only location.
							var requestLocationUrl = new Uri(RequestUrl, location);
							var responseLocationUrl = new UriBuilder(requestLocationUrl);

							// if the requested location for the host and port is the same as the requested URL we need to update them to the response
							if (Uri.Compare(requestLocationUrl, RequestUrl, UriComponents.SchemeAndServer, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) == 0) {
								responseLocationUrl.Port = ResponseUrl.Port;
								responseLocationUrl.Host = ResponseUrl.Host;
								responseLocationUrl.Scheme = ResponseUrl.Scheme;

								string path = responseLocationUrl.Path;
								int pathIndex = path.IndexOf(RequestUrl.AbsolutePath, StringComparison.OrdinalIgnoreCase);

								// since this is not redirecting to a different server try to replease the path
								if (pathIndex > -1) {
									path = path.Remove(pathIndex, RequestUrl.AbsolutePath.Length);
									path = path.Insert(pathIndex, ResponseUrl.AbsolutePath);

									responseLocationUrl.Path = path;
								}
							}

							context.Response.AppendHeader(name, responseLocationUrl.Uri.OriginalString);
						}
					} catch { /* do nothing on purpose */ }

					// nothing else can occure just continue processing from next header
					continue;
				}

				// if this is a chuncked response then we should send it correctly
				if (name == "Transfer-Encoding") {
					/* http://www.go-mono.com/docs/index.aspx?link=P%3aSystem.Web.HttpResponse.Buffer
					 * 
					 * This controls whether HttpResponse should buffer the output before it is delivered to a 
					 * client. The default is true.
					 * 
					 * The buffering can be changed during the execution back and forth if needed. Notice that 
					 * changing the buffering state will not flush the current contents held in the output buffer, 
					 * the contents will only be flushed out on the next write operation or by manually calling 
					 * System.Web.HttpResponse.Flush
					 */
					context.Response.BufferOutput = false;
					continue;
				}

				if (name == "Content-Type") {
					/* http://www.w3.org/Protocols/rfc1341/7_2_Multipart.html
					 * http://en.wikipedia.org/wiki/Motion_JPEG#M-JPEG_over_HTTP
					 * 
					 * The multipart/x-mixed-replace content-type should be treated as a streaming content and shouldn't be buffered.
					 */
					if (values[0].StartsWith("multipart/x-mixed-replace"))
						context.Response.BufferOutput = false;
				}

				// it is nessisary to get the values for headers that are allowed to specifiy
				// multiple values in an instance (i.e. Set-Cookie)
				foreach (string value in values)
					context.Response.AppendHeader(name, value);
			}

			Manager.Log(String.Format("Response is {0}being buffered", (context.Response.BufferOutput ? "" : "not ")), "Proxy");

			// add the vanity url to the header
			Manager.TryToAddVanityHeader(new HttpContextWrapper(context));

			// set all HTTP specific protocol stuff
			context.Response.StatusCode = (int)response.StatusCode;

			Manager.Log(String.Format("Responding '{0}'", ((int)response.StatusCode)), "Proxy");

			await OnResponseToClient(context, response);

			await response.Content.CopyToAsync(context.Response.OutputStream);
		}

		/// <summary>
		/// Called when [request to target].
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="request">The request.</param>
		protected async virtual Task OnRequestToTarget(HttpContext context, HttpRequestMessage request)
		{
		}

		/// <summary>
		/// Called when [response to client].
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="response">The response.</param>
		protected async virtual Task OnResponseToClient(HttpContext context, HttpResponseMessage response)
		{
		}
#else
		#region IHttpAsyncHandler Members

		/// <summary>
		/// Initiates an asynchronous call to the HTTP handler.
		/// </summary>
		/// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
		/// <param name="cb">The <see cref="T:System.AsyncCallback"/> to call when the asynchronous method call is complete. If <paramref name="cb"/> is null, the delegate is not called.</param>
		/// <param name="extraData">Any extra data needed to process the request.</param>
		/// <returns>
		/// An <see cref="T:System.IAsyncResult"/> that contains information about the status of the process.
		/// </returns>
		public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
		{
			_proxyDelegate = new Action<HttpContext>(ProcessRequest);
			return _proxyDelegate.BeginInvoke(context, cb, extraData);
		}

		/// <summary>
		/// Provides an asynchronous process End method when the process ends.
		/// </summary>
		/// <param name="result">An <see cref="T:System.IAsyncResult"/> that contains information about the status of the process.</param>
		public void EndProcessRequest(IAsyncResult result)
		{
			if (result == null)
				throw new ArgumentNullException("result");

			if (_proxyDelegate == null)
				throw new InvalidOperationException("A async scan was never started.");

			try
			{
				_proxyDelegate.EndInvoke(result);
			}
			finally
			{
				_proxyDelegate = null;
			}
		}

		#endregion
#endif
	}
}