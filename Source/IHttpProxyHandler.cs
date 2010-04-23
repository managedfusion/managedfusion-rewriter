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
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace ManagedFusion.Rewriter
{
	public interface IHttpProxyHandler : IHttpHandler
	{
		/// <summary>
		/// Inits the specified request for the proxy.
		/// </summary>
		/// <param name="requestUrl">The request URL.</param>
		/// <param name="responseUrl">The response URL.</param>
		void Init(Uri requestUrl, Uri responseUrl);

		/// <summary>
		/// Gets or sets the requested URL.
		/// </summary>
		/// <value>The requested URL.</value>
		Uri RequestUrl { get; }

		/// <summary>
		/// Gets or sets the response URL.
		/// </summary>
		/// <value>The response URL.</value>
		Uri ResponseUrl { get; }
	}
}
