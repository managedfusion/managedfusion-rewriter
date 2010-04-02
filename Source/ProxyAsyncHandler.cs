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
using System.Web;
using System.Diagnostics;


namespace ManagedFusion.Rewriter
{
	public class ProxyAsyncHandler : ProxyHandler, IHttpAsyncHandler
	{
		[NonSerialized, DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<HttpContext> _proxyDelegate;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProxyHandler"/> class.
		/// </summary>
		public ProxyAsyncHandler()
		{
		}

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
	}
}