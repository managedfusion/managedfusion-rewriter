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
	public class QueryStringAppendFlag : IRuleFlag
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="QueryStringAppendFlag"/> class.
		/// </summary>
		public QueryStringAppendFlag()
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
			context.SubstitutedUrl = AppendQueryString(context.SubstitutedUrl, context.CurrentUrl);

			return RuleFlagProcessorResponse.ContinueToNextFlag;
		}

		#endregion

		/// <summary>
		/// Appends the query string.
		/// </summary>
		/// <param name="substituedUrl">The substitued URL.</param>
		/// <param name="existingUrl">The existing URL.</param>
		/// <returns></returns>
		private Uri AppendQueryString(Uri substituedUrl, Uri existingUrl)
		{
			string append = existingUrl.Query.TrimStart('?');

			if (!String.IsNullOrEmpty(append))
			{
				UriBuilder builder = new UriBuilder(substituedUrl);

				if (String.IsNullOrEmpty(builder.Query) || builder.Query == "?")
					builder.Query = append;
				else
					builder.Query = builder.Query.TrimStart('?') + "&" + append;

				return builder.Uri;
			}

			return substituedUrl;
		}
	}
}
