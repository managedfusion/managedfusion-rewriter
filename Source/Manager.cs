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
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using ManagedFusion.Rewriter.Configuration;
using ManagedFusion.Rewriter.Engines;
using System.Diagnostics;

namespace ManagedFusion.Rewriter
{
	/// <summary>
	/// 
	/// </summary>
	internal static class Manager
	{
		private static object _logLock = new object();
		private static string _logPath;
		private static ApplicationEngine _applicationEngine;
		private static IRewriterEngine _rewriterEngine;

		public static readonly RegexOptions RuleOptions = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant;

		/// <summary>
		/// The rewriter name.
		/// </summary>
		public const string RewriterName = "ManagedFusion";

		/// <summary>
		/// The rewriter user agent name.
		/// </summary>
		public const string RewriterUserAgent = "ManagedFusion (rewriter; reverse-proxy; +http://managedfusion.com/)";

		/// <summary>
		/// The name of the stoarge location in HttpContext.Items for the proxy handler.
		/// </summary>
		public const string ProxyHandlerStorageName = "ManagedFusion.Rewriter.ProxyHandler";

		/// <summary>
		/// Gets or sets the rewriter version.
		/// </summary>
		/// <value>The rewriter version.</value>
		public static Version RewriterVersion
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the type of the proxy.
		/// </summary>
		/// <value>The type of the proxy.</value>
		public static Type ProxyType
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the type of the proxy async.
		/// </summary>
		/// <value>The type of the proxy async.</value>
		public static Type ProxyAsyncType
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the rewriter name and version.
		/// </summary>
		/// <value>The rewriter name and version.</value>
		public static string RewriterNameAndVersion
		{
			get { return RewriterName + "/" + RewriterVersion.ToString(2); }
		}

		/// <summary>
		/// Gets the configuration.
		/// </summary>
		/// <value>The configuration.</value>
		public static ManagedFusionRewriterSectionGroup Configuration
		{
			get { return ManagedFusionRewriterSectionGroup.Instance; }
		}

		/// <summary>
		/// Gets a value indicating whether [application rules need loading].
		/// </summary>
		/// <value>
		/// 	<see langword="true"/> if [application rules need loading]; otherwise, <see langword="false"/>.
		/// </value>
		public static bool ApplicationRulesNeedLoading
		{
			get { return _applicationEngine == null; }
		}

		#region Logging

		/// <summary>
		/// Gets or sets a value indicating if logging is enabled.
		/// </summary>
		public static bool LogEnabled { get; set; }

		/// <summary>
		/// Gets or sets the log path.
		/// </summary>
		public static string LogPath
		{
			get { return _logPath; }
			set
			{
				if (String.IsNullOrEmpty(value))
				{
					_logPath = null;
					return;
				}

				try
				{
					StreamWriter sw = File.Exists(value) ? File.AppendText(value) : File.CreateText(value);
					sw.WriteLine("**********************************************************************************");
					sw.WriteLine("Logging started on " + DateTime.Now.ToString("s"));
					sw.WriteLine("**********************************************************************************");

					sw.Flush();
					sw.Close();
					sw.Dispose();

					_logPath = value;
				}
				catch (Exception exc)
				{
					throw new RewriterException("Problem with log file " + value, exc);
				}
			}
		}

		/// <summary>
		/// Log the message and category if the condition is met.
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="messaage"></param>
		/// <param name="category"></param>
		public static void LogIf(bool condition, string messaage, string category)
		{
			if (condition)
				Log(messaage, category);
		}

		/// <summary>
		/// Log the message if the condition is met.
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="message"></param>
		public static void LogIf(bool condition, string message)
		{
			if (condition)
				Log(message, null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public static void Log(string message)
		{
			Log(message, null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="category"></param>
		public static void Log(string message, string category)
		{
			if (!LogEnabled)
				return;

			if (_logPath == null)
				return;

			StringBuilder sb = new StringBuilder();
			sb.Append(DateTime.Now.ToString("s"));
			sb.Append(" ");

			if (category != null)
			{
				sb.Append("[");
				sb.Append(category);
				sb.Append("] ");
			}

			sb.Append(message);
			sb.Append(Environment.NewLine);

			lock (_logLock)
			{
				File.AppendAllText(_logPath, sb.ToString(), Encoding.UTF8);
			}

			if (Configuration.Rewriter.TraceLog)
				Trace.WriteLine(message, category);
		}

		#endregion

		/// <summary>
		/// Loads the application rules.
		/// </summary>
		/// <param name="args">The <see cref="ManagedFusion.Rewriter.LoadRulesEventArgs"/> instance containing the event data.</param>
		public static void LoadApplicationRules(LoadRulesEventArgs args)
		{
			_applicationEngine = new ApplicationEngine(args);
			_applicationEngine.Init();
		}

		/// <summary>
		/// Clears the application rules.
		/// </summary>
		public static void ClearApplicationRules()
		{
			_applicationEngine.RefreshRules();
			_applicationEngine = null;
		}

		/// <summary>
		/// Runs the rules.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public static Uri RunRules(HttpContext context)
		{
			Uri url = _applicationEngine.RunRules(context);

			if (url == null)
				url = _rewriterEngine.RunRules(context);

			return url;
		}

		/// <summary>
		/// Runs the output rules.
		/// </summary>
		/// <param name="context">The context.</param>
		public static void RunOutputRules(HttpContext context)
		{
			_applicationEngine.RunOutputRules(context);
			_rewriterEngine.RunOutputRules(context);
		}

		#region Headers and Server Variables

		/// <summary>
		/// Adds the response header.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		public static void AddResponseHeader(HttpContext context, string name, string value)
		{
			context.Response.AppendHeader(name, value);
		}

		/// <summary>
		/// Trys to add the response header.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		/// <remarks>This was added to protect against the <c>Server cannot append header after HTTP headers have been sent.</c> exception that occurs occationally 
		/// when a module before the rewriter preforms a <see cref="HttpResponse.Flush"/> forcing all the headers to be written before other modules have had a chance.</remarks>
		/// <returns></returns>
		public static bool TryAddResponseHeader(HttpContext context, string name, string value)
		{
			try
			{
				AddResponseHeader(context, name, value);
				return true;
			}
			catch (Exception exc)
			{
				Manager.Log(exc.Message, "Error");
				return false;
			}
		}

		/// <summary>
		/// Adds the request header.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="name">The header name.</param>
		/// <param name="value">The value.</param>
		public static void AddRequestHeader(HttpContext context, string name, string value)
		{
			// also store header in context
			context.Items[name] = value;

			if (Configuration.Rewriter.AllowRequestHeaderAdding && context.Request.Headers[name] == null)
			{
				try
				{
					if (HttpRuntime.UsingIntegratedPipeline)
					{
						context.Request.Headers.Add(name, value);
					}
					else
					{
						Type targetType = typeof(NameValueCollection);

						// get the property for setting readability
						PropertyInfo isReadOnlyProperty = targetType.GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);

						// set headers as read and write
						isReadOnlyProperty.SetValue(context.Request.Headers, false, null);

						ArrayList list = new ArrayList();
						list.Add(value);

						// get the method to fill in the headers
						MethodInfo fillInHeadersCollectionMethod = targetType.GetMethod("BaseSet", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(string), typeof(object) }, null);
						fillInHeadersCollectionMethod.Invoke(context.Request.Headers, new object[] { name, list });

						// set headers as read only
						isReadOnlyProperty.SetValue(context.Request.Headers, true, null);
					}
				}
				catch (SecurityException) { }
				catch (MethodAccessException) { }
				catch (NullReferenceException) { }
			}
		}

		/// <summary>
		/// Set the server variable.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="name">The server variable name.</param>
		/// <param name="value">The value.</param>
		public static void SetServerVariable(HttpContext context, string name, string value, bool replace)
		{
			if (Configuration.Rewriter.AllowServerVariableSetting)
			{
				try
				{
					// if a replace isn't allowed and there is already a server variable set then exit
					if (!replace && !String.IsNullOrEmpty(context.Request.ServerVariables.Get(name)))
						return;

					if (HttpRuntime.UsingIntegratedPipeline)
					{
						context.Request.ServerVariables.Set(name, value);
					}
					else
					{
						Type targetType = typeof(NameValueCollection);

						// get the property for setting readability
						PropertyInfo isReadOnlyProperty = targetType.GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);

						// set headers as read and write
						isReadOnlyProperty.SetValue(context.Request.Headers, false, null);

						ArrayList list = new ArrayList();
						list.Add(value);

						// get the method to fill in the headers
						MethodInfo fillInHeadersCollectionMethod = targetType.GetMethod("BaseSet", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(string), typeof(object) }, null);
						fillInHeadersCollectionMethod.Invoke(context.Request.Headers, new object[] { name, list });

						// set headers as read only
						isReadOnlyProperty.SetValue(context.Request.Headers, true, null);
					}
				}
				catch (SecurityException) { }
				catch (MethodAccessException) { }
				catch (NullReferenceException) { }
			}
		}

		/// <summary>
		/// Tries to add X rewrite URL header.
		/// </summary>
		/// <param name="context">The context.</param>
		public static void TryToAddXRewriteUrlHeader(HttpContext context)
		{
			// add the X-Rewrite-Url to the context items because it is needed
			// for other services offered by this rewriter, it just won't be 
			// available through the header for this request
			context.Items["X-Rewrite-Url"] = context.Request.RawUrl;

			// add the original url to the header
			if (Configuration.Rewriter.AllowXRewriteUrlHeader)
				AddRequestHeader(context, "X-Rewrite-Url", HttpUtility.UrlPathEncode(context.Request.RawUrl));
		}

		/// <summary>
		/// Tries to add vanity header.
		/// </summary>
		/// <param name="context">The context.</param>
		public static void TryToAddVanityHeader(HttpContext context)
		{
			// add the vanity url to the header
			if (Configuration.Rewriter.AllowVanityHeader)
			{
				TryAddResponseHeader(context, "X-Rewritten-By", RewriterUserAgent);
				TryAddResponseHeader(context, "X-ManagedFusion-Rewriter-Version", RewriterVersion.ToString(2));
			}
		}

		#endregion

		#region ConfigureIIS7WorkerRequest

		private static readonly Type IIS7WorkerRequestType = Type.GetType("System.Web.Hosting.IIS7WorkerRequest, System.Web");
		private static readonly Type ISAPIWorkerRequestInProcForIIS7Type = Type.GetType("System.Web.Hosting.ISAPIWorkerRequestInProcForIIS7, System.Web");

		/// <summary>
		/// Configure IIS7 Worker Request for rewriting.
		/// </summary>
		/// <param name="context">The context.</param>
		public static void ModifyIIS7WorkerRequest(HttpContext context)
		{
			if (Configuration.Rewriter.ModifyIIS7WorkerRequest && HttpRuntime.UsingIntegratedPipeline)
			{
				try
				{
					HttpWorkerRequest wr = ((IServiceProvider)context).GetService(typeof(HttpWorkerRequest)) as HttpWorkerRequest;
					Type wrType = wr.GetType();

					if (wr != null && (wrType == IIS7WorkerRequestType || wrType == ISAPIWorkerRequestInProcForIIS7Type))
					{
						// get the field for setting if the rewrite module is enabled
						FieldInfo isRewriteModuleEnabledField = wrType.GetField("_isRewriteModuleEnabled", BindingFlags.Instance | BindingFlags.NonPublic);

						// set the rewrite module to true for rewriter enabled
						isRewriteModuleEnabledField.SetValue(wr, true);
					}
				}
				catch (SecurityException) { }
				catch (MethodAccessException) { }
				catch (NullReferenceException) { }
			}
		}

		#endregion

		/// <summary>
		/// Redirects the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="type">The type.</param>
		/// <param name="url">The URL.</param>
		public static void Redirect(HttpContext context, string type, Uri url)
		{
			Log(type + " redirect occured", "Response");

			context.Response.Clear();

			TryToAddXRewriteUrlHeader(context);
			TryToAddVanityHeader(context);

			switch (type)
			{
				case "permanent":
				case "301":
					context.Response.StatusCode = (int)HttpStatusCode.MovedPermanently;
					context.Response.StatusDescription = "Moved Permanently";
					break;

				case "found":
				case "302":
					context.Response.StatusCode = (int)HttpStatusCode.Found;
					context.Response.StatusDescription = "Found";
					break;

				case "seeother":
				case "303":
					context.Response.StatusCode = (int)HttpStatusCode.SeeOther;
					context.Response.StatusDescription = "See Other";
					break;

				case "304":
					context.Response.StatusCode = (int)HttpStatusCode.NotModified;
					context.Response.StatusDescription = "Not Modified";
					break;

				case "temp":
				case "307":
					context.Response.StatusCode = (int)HttpStatusCode.TemporaryRedirect;
					context.Response.StatusDescription = "Temporary Redirect";
					break;

				default:
					goto case "found";
			}

			context.Response.RedirectLocation = url.ToString();
			context.Response.ContentType = "text/html";

			context.Response.Write("<html><head><title>");
			context.Response.Write(context.Response.StatusDescription);
			context.Response.Write("</title></head><body><p>The URI that you requested has been <a href=\"");
			context.Response.Write(HttpUtility.HtmlAttributeEncode(url.ToString()));
			context.Response.Write("\">moved to here</a>.</p></body></html>");

			context.Response.End();
		}

		/// <summary>
		/// Initializes the <see cref="Manager"/> class.
		/// </summary>
		static Manager()
		{
			// gets the assembly version of the rewriter
			RewriterVersion = typeof(Manager).Assembly.GetName().Version.Clone() as Version;

			try
			{
				// setup the rewriter proxy
				ProxyType = Type.GetType(Configuration.Rewriter.Proxy.ProxyType);

				// setup the async rewriter proxy
				ProxyAsyncType = Type.GetType(Configuration.Rewriter.Proxy.ProxyAsyncType);
			}
			catch
			{
				ProxyType = typeof(ProxyHandler);
				ProxyAsyncType = typeof(ProxyAsyncHandler);
			}
			finally
			{
				if (ProxyType == null)
					ProxyType = typeof(ProxyHandler);

				if (ProxyAsyncType == null)
					ProxyAsyncType = typeof(ProxyAsyncHandler);
			}

			// load engine
			switch (Configuration.Rules.Engine)
			{
				case RulesEngine.Apache:
					_rewriterEngine = new ApacheEngine();
					break;

				case RulesEngine.Microsoft:
					_rewriterEngine = new MicrosoftEngine();
					break;

				case RulesEngine.Other:
					{
						try
						{
							_rewriterEngine = (IRewriterEngine)Activator.CreateInstance(Type.GetType(Configuration.Rules.EngineType));
							break;
						}
						catch (Exception exc)
						{
							throw new RewriterEngineException("The engine specified in the web.config cannot be found, " + Configuration.Rules.EngineType, exc);
						}
					}
			}

			// init engine
			_rewriterEngine.Init();
		}
	}
}