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

using System.Web;

namespace ManagedFusion.Rewriter.Rules.Flags
{
	public class ResponseMimeTypeFlag : IRuleFlag
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ResponseStatusFlag"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		public ResponseMimeTypeFlag(string type)
		{
			Type = type;
		}

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		public string Type { get; private set; }

		#region IRuleFlag Members

		/// <summary>
		/// Applies the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public RuleFlagProcessorResponse Apply(RuleContext context)
		{
			SetMimeType(context.HttpContext, Type);

			Manager.LogIf(context.LogLevel >= 2, "Content Type: " + Type, "Rewrite");
			return RuleFlagProcessorResponse.ContinueToNextFlag;
		}

		#endregion

		/// <summary>
		/// Sets the type of the MIME.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="type">The type.</param>
		private void SetMimeType(HttpContextBase context, string type)
		{
			context.Response.ContentType = type;
		}
	}
}