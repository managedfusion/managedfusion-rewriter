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

namespace ManagedFusion.Rewriter
{
	/// <summary>
	/// 
	/// </summary>
	public class RuleSetContext
	{
		private byte[] _responseContent;

		/// <summary>
		/// Initializes a new instance of the <see cref="RuleSetContext"/> class.
		/// </summary>
		/// <param name="copy">The copy.</param>
		internal RuleSetContext(RuleSetContext copy)
		{
			if (copy == null)
				throw new ArgumentNullException("copy");

			RuleSet = copy.RuleSet;
			RequestedUrl = copy.RequestedUrl;
			_responseContent = copy._responseContent;
			HttpContext = copy.HttpContext;
			IsOutputRuleSet = copy.IsOutputRuleSet;
			LogCategory = String.Empty;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RuleSetContext"/> class.
		/// </summary>
		/// <param name="ruleSet">The rule set.</param>
		/// <param name="requestedUrl">The requested URL.</param>
		/// <param name="httpContext">The HTTP context.</param>
		public RuleSetContext(RuleSet ruleSet, Uri requestedUrl, HttpContextBase httpContext)
		{
			if (ruleSet == null)
				throw new ArgumentNullException("ruleSet");

			if (requestedUrl == null)
				throw new ArgumentNullException("requestedUrl");

			if (httpContext == null)
				throw new ArgumentNullException("httpContext");

			RuleSet = ruleSet;
			_responseContent = null;
			RequestedUrl = requestedUrl;
			HttpContext = httpContext;
			IsOutputRuleSet = false;
			LogCategory = String.Empty;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RuleSetContext" /> class.
		/// </summary>
		/// <param name="ruleSet">The rule set.</param>
		/// <param name="responseContent">The response content.</param>
		/// <param name="httpContext">The HTTP context.</param>
		public RuleSetContext(RuleSet ruleSet, byte[] responseContent, HttpContextBase httpContext)
		{
			if (ruleSet == null)
				throw new ArgumentNullException("ruleSet");

			if (responseContent == null)
				throw new ArgumentNullException("responseContent");

			if (httpContext == null)
				throw new ArgumentNullException("httpContext");

			RuleSet = ruleSet;
			_responseContent = responseContent;
			RequestedUrl = httpContext.Request.Url;
			HttpContext = httpContext;
			IsOutputRuleSet = true;
			LogCategory = String.Empty;
		}

		/// <summary>
		/// Checks if the rule set is running through the output rules.
		/// </summary>
		public bool IsOutputRuleSet { get; private set; }

		/// <summary>
		/// Gets the log level.
		/// </summary>
		/// <value>The log level.</value>
		public int LogLevel
		{
			get { return RuleSet.LogLevel; }
		}

		/// <summary>
		/// Gets the log category.
		/// </summary>
		/// <value>The log category.</value>
		public string LogCategory { get; protected set; }

		/// <summary>
		/// Gets or sets the rule set.
		/// </summary>
		/// <value>The rule set.</value>
		public RuleSet RuleSet { get; private set; }

		/// <summary>
		/// Gets or sets the requested URL.
		/// </summary>
		/// <value>The requested URL.</value>
		public Uri RequestedUrl { get; private set; }

		/// <summary>
		/// Gets or sets the response content.
		/// </summary>
		public byte[] ResponseContent
		{
			get
			{
				byte[] output = new byte[_responseContent.LongLength];
				Array.Copy(_responseContent, output, _responseContent.LongLength);
				return output;
			}
		}

		/// <summary>
		/// Gets or sets the HTTP context.
		/// </summary>
		/// <value>The HTTP context.</value>
		public HttpContextBase HttpContext { get; private set; }
	}
}
