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

namespace ManagedFusion.Rewriter.Rules.Flags
{
	public class ProxyFlag : IRuleFlag
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProxyFlag"/> class.
		/// </summary>
		public ProxyFlag()
		{
		}

		#region IRuleFlag Members

		/// <summary>
		/// Applies the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public RuleFlagProcessorResponse Apply(RuleContext context)
		{
			IHttpProxyHandler proxyHandler = null;

			if (Manager.Configuration.Rewriter.Proxy.UseAsyncProxy)
				proxyHandler = Activator.CreateInstance(Manager.ProxyAsyncType) as IHttpProxyHandler;
			else
				proxyHandler = Activator.CreateInstance(Manager.ProxyType) as IHttpProxyHandler;

			if (proxyHandler != null)
				proxyHandler.Init(context.SubstitutedUrl, context.RequestedUrl);

			context.HttpContext.Items.Add(Manager.ProxyHandlerStorageName, proxyHandler);
			Manager.LogIf(context.LogLevel >= 1, "Proxy: " + context.SubstitutedUrl, "Rewrite");
			
			return RuleFlagProcessorResponse.ExitRuleSet;
		}

		#endregion
	}
}
