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
using System.Security.Permissions;
using System.Web;
using System.Collections.Specialized;


namespace ManagedFusion.Rewriter
{
	/// <summary>
	/// 
	/// </summary>
	public class RewriterModule : IHttpModule
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RewriterModule"/> class.
		/// </summary>
		public RewriterModule()
		{
		}

		/// <summary>
		/// Occurs when [load application rules].
		/// </summary>
		public event EventHandler<LoadRulesEventArgs> LoadApplicationRules;

		/// <summary>
		/// Raises the <see cref="E:LoadApplicationRules"/> event.
		/// </summary>
		/// <param name="e">The <see cref="ManagedFusion.Rewriter.LoadRulesEventArgs"/> instance containing the event data.</param>
		protected virtual void OnLoadApplicationRules(LoadRulesEventArgs e)
		{
			if (LoadApplicationRules != null)
				LoadApplicationRules(this, e);
		}

		/// <summary>
		/// Occurs when [refresh application rules].
		/// </summary>
		public event EventHandler<RefreshRulesEventArgs> RefreshApplicationRules;

		/// <summary>
		/// Raises the <see cref="E:RefreshApplicationRules"/> event.
		/// </summary>
		/// <param name="e">The <see cref="ManagedFusion.Rewriter.RefreshRulesEventArgs"/> instance containing the event data.</param>
		protected virtual void OnRefreshApplicationRules(RefreshRulesEventArgs e)
		{
			if (RefreshApplicationRules != null)
				RefreshApplicationRules(this, e);
		}

		#region IHttpModule Members

		/// <summary>
		/// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"></see>.
		/// </summary>
		public void Dispose()
		{
		}

		/// <summary>
		/// Initializes a module and prepares it to handle requests.
		/// </summary>
		/// <param name="context">An <see cref="T:System.Web.HttpApplication"></see> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application</param>
		[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
		public void Init(HttpApplication context)
		{
			context.BeginRequest += new EventHandler(context_BeginRequest);

			context.PostResolveRequestCache += new EventHandler(context_PostResolveRequestCache);
			context.PostMapRequestHandler += new EventHandler(context_PostMapRequestHandler);

			context.PostRequestHandlerExecute += new EventHandler(context_PostRequestHandlerExecute);
			context.PostReleaseRequestState += new EventHandler(context_PostReleaseRequestState);
		}

		#endregion

		/// <summary>
		/// Handles the Begin event of the request control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void context_BeginRequest(object sender, EventArgs e)
		{
			#region LoadApplicationRules Event

			if (Manager.ApplicationRulesNeedLoading)
			{
				// call load application rules
				LoadRulesEventArgs loadRulesArgs = new LoadRulesEventArgs();
				OnLoadApplicationRules(loadRulesArgs);

				Manager.LoadApplicationRules(loadRulesArgs);
			}

			#endregion

			HttpContext context = ((HttpApplication)sender).Context;
			Uri rewrittenUrl = Manager.RunRules(context);

			// make sure the rewrittenUrl returned is not null
			// a null value can indicate a proxy request
			if (rewrittenUrl != null)
			{
				string rawUrl = context.Request.RawUrl;

				// configure IIS7 for worker request
				// this will not do anything if the IIS7 worker request is not found
				Manager.ModifyIIS7WorkerRequest(context);

				// get the path and query for the rewritten URL
				string rewrittenUrlPath = rewrittenUrl.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped);

				// rewrite path using new URL
				if (HttpRuntime.UsingIntegratedPipeline && Manager.Configuration.Rewriter.AllowIis7TransferRequest)
				{
					HttpRequest request = context.Request;
					NameValueCollection headers = new NameValueCollection(context.Request.Headers);

					int transferCount = 1;
					string transferCountHeader = headers["X-Rewriter-Transfer"];
					if (!String.IsNullOrEmpty(transferCountHeader) && Int32.TryParse(transferCountHeader, out transferCount))
						transferCount++;

					headers["X-Rewriter-Transfer"] = transferCount.ToString(); 

					context.Server.TransferRequest(
						rewrittenUrlPath,
						true,
						request.HttpMethod,
						headers
					);
				}
				else
					context.RewritePath(rewrittenUrlPath, Manager.Configuration.Rewriter.RebaseClientPath);

				// set the server variables to the correct header based on the rawUrl
				//Manager.SetServerVariable(context, "URL", rawUrl);
			}
		}

		/// <summary>
		/// Handles the PostResolveRequestCache event of the context control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void context_PostResolveRequestCache(object sender, EventArgs e)
		{
			HttpContext context = ((HttpApplication)sender).Context;

			// check to see if this is a proxy request
			if (context.Items.Contains(Manager.ProxyHandlerStorageName))
				context.RewritePath("~/RewriterProxy.axd");
		}

		/// <summary>
		/// Handles the PostMapRequestHandler event of the context control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void context_PostMapRequestHandler(object sender, EventArgs e)
		{
			HttpContext context = ((HttpApplication)sender).Context;

			// check to see if this is a proxy request
			if (context.Items.Contains(Manager.ProxyHandlerStorageName))
			{
				IHttpProxyHandler proxy = context.Items[Manager.ProxyHandlerStorageName] as IHttpProxyHandler;

				context.RewritePath("~" + proxy.ResponseUrl.PathAndQuery);
				context.Handler = proxy;
			}
		}

		/// <summary>
		/// Handles the PostHandlerExecute event of the request control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void context_PostRequestHandlerExecute(object sender, EventArgs e)
		{
			#region RefreshApplicationRules Event

			RefreshRulesEventArgs refreshRulesArgs = new RefreshRulesEventArgs();
			OnRefreshApplicationRules(refreshRulesArgs);

			// if refresh rules is set to true then clear out the application rules
			// which will force the LoadApplicationRules event to fire for the next
			// request
			if (refreshRulesArgs.RefreshRules)
				Manager.ClearApplicationRules();

			#endregion
		}

		/// <summary>
		/// Handles the PostReleaseRequestState event of the context control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void context_PostReleaseRequestState(object sender, EventArgs e)
		{
			if (Manager.Configuration.Rules.AllowOutputProcessing)
			{
				HttpContext context = ((HttpApplication)sender).Context;
				Manager.RunOutputRules(context);
			}
		}
	}
}