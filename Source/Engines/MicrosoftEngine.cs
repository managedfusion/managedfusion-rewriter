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
using System.Web.Hosting;
using System.IO;

namespace ManagedFusion.Rewriter.Engines
{
	/// <summary>
	/// 
	/// </summary>
	internal class MicrosoftEngine : IRewriterEngine
	{
		private MicrosoftRuleSet _ruleSet;

		/// <summary>
		/// Initializes a new instance of the <see cref="MicrosoftEngine"/> class.
		/// </summary>
		public MicrosoftEngine()
		{
		}

		/// <summary>
		/// Refreshes the config.
		/// </summary>
		private void RefreshConfig()
		{
			string configurationPath = HostingEnvironment.MapPath("/web.config");
			var configurationFile = new FileInfo(configurationPath);

			_ruleSet = new MicrosoftRuleSet(configurationFile);
		}

		#region IRewriterEngine Members

		/// <summary>
		/// Inits this instance.
		/// </summary>
		public void Init()
		{
			RefreshConfig();
		}

		/// <summary>
		/// Refreshes the rules.
		/// </summary>
		public void RefreshRules()
		{
			RefreshConfig();
		}

		/// <summary>
		/// Runs the rules.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public Uri RunRules(HttpContext context)
		{
			Uri url = new Uri(context.Request.Url, context.Request.RawUrl);
			Uri rewritenUrl = _ruleSet.RunRules(context, url);

			return rewritenUrl;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		public void RunOutputRules(HttpContext context)
		{
			if (_ruleSet.OutputRuleCount > 0)
				context.Response.Filter = new RuleSetOutputFilter(context.Response.Filter, context, _ruleSet);
		}

		#endregion
	}
}
