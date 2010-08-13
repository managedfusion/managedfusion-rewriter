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

namespace ManagedFusion.Rewriter
{
	/// <summary>
	/// 
	/// </summary>
	internal class KnownHttpVerb
	{
		private static readonly Dictionary<string, KnownHttpVerb> NamedHeaders;

		/// <summary>
		/// Initializes the <see cref="KnownHttpVerb"/> class.
		/// </summary>
		static KnownHttpVerb()
		{
			NamedHeaders = new Dictionary<string, KnownHttpVerb>(StringComparer.OrdinalIgnoreCase) {
			    {"GET", new KnownHttpVerb("GET", false, true, false, false)},
			    {"POST", new KnownHttpVerb("POST", true, false, false, false)},
			    {"HEAD", new KnownHttpVerb("HEAD", false, true, false, true)},
			    {"CONNECT", new KnownHttpVerb("CONNECT", false, true, true, false)},
			    {"PUT", new KnownHttpVerb("PUT", true, false, false, false)}
			};
		}

		/// <summary>
		/// Parses the specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public static KnownHttpVerb Parse(string name)
		{
			KnownHttpVerb verb;

			if (!NamedHeaders.TryGetValue(name, out verb))
				verb = new KnownHttpVerb(name, false, false, false, false);

			return verb;
		}

		public string Name;
		public bool RequireContentBody;
		public bool ContentBodyNotAllowed;
		public bool ConnectRequest;
		public bool ExpectNoContentResponse;

		/// <summary>
		/// Initializes a new instance of the <see cref="KnownHttpVerb"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="requireContentBody">if set to <c>true</c> [require content body].</param>
		/// <param name="contentBodyNotAllowed">if set to <c>true</c> [content body not allowed].</param>
		/// <param name="connectRequest">if set to <c>true</c> [connect request].</param>
		/// <param name="expectNoContentResponse">if set to <c>true</c> [expect no content response].</param>
		private KnownHttpVerb(string name, bool requireContentBody, bool contentBodyNotAllowed, bool connectRequest, bool expectNoContentResponse)
		{
			Name = name;
			RequireContentBody = requireContentBody;
			ContentBodyNotAllowed = contentBodyNotAllowed;
			ConnectRequest = connectRequest;
			ExpectNoContentResponse = expectNoContentResponse;
		}

		/// <summary>
		/// Equalses the specified verb.
		/// </summary>
		/// <param name="verb">The verb.</param>
		/// <returns></returns>
		public bool Equals(KnownHttpVerb verb)
		{
			if (this != verb)
				return String.Compare(Name, verb.Name, StringComparison.OrdinalIgnoreCase) == 0;

			return true;
		}
	}
}
