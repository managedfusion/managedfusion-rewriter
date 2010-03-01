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

namespace ManagedFusion.Rewriter.Rules.Flags
{
	public class ResponseCookieFlag : IRuleFlag
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ResponseCookieFlag"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		/// <param name="domain">The domain.</param>
		/// <param name="expires">The expires.</param>
		/// <param name="path">The path.</param>
		public ResponseCookieFlag(string name, string value, string domain, TimeSpan? expires, string path)
		{
			Name = name;
			Value = value;
			Domain = domain;
			Expires = expires;
			Path = path;
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; private set; }

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public string Value { get; private set; }

		/// <summary>
		/// Gets or sets the domain.
		/// </summary>
		/// <value>The domain.</value>
		public string Domain { get; private set; }

		/// <summary>
		/// Gets or sets the expires.
		/// </summary>
		/// <value>The expires.</value>
		public TimeSpan? Expires { get; private set; }

		/// <summary>
		/// Gets or sets the path.
		/// </summary>
		/// <value>The path.</value>
		public string Path { get; private set; }

		#region IRuleFlag Members

		/// <summary>
		/// Applies the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public RuleFlagProcessorResponse Apply(RuleContext context)
		{
			var httpContext = context.HttpContext;

			HttpCookie cookie = new HttpCookie(Name, Value);
			cookie.Domain = Domain;

			if (Expires.HasValue)
				cookie.Expires = (DateTime.Now + Expires.Value);

			if (String.IsNullOrEmpty(Path))
				cookie.Path = Path;

			httpContext.Response.Cookies.Add(cookie);

			Manager.LogIf(context.LogLevel >= 2, "Cookie: " + Name + ":" + Value, "Rewrite");
			return RuleFlagProcessorResponse.ContinueToNextFlag;
		}

		#endregion
	}
}
