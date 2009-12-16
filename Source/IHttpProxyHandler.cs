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
