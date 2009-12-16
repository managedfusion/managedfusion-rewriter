/** 
 * Copyright (C) 2007-2008 Nicholas Berardi, Managed Fusion, LLC (nick@managedfusion.com)
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
	public sealed class LoadRulesEventArgs : EventArgs
	{
		private IList<IRule> _rules;
		private string _base;

		/// <summary>
		/// Initializes a new instance of the <see cref="LoadRulesEventArgs"/> class.
		/// </summary>
		public LoadRulesEventArgs()
		{
			_rules = new List<IRule>();
			_base = "/";
		}

		/// <summary>
		/// Gets the base.
		/// </summary>
		/// <value>The base.</value>
		public string Base
		{
			get	{ return _base; }
			set
			{
				if (value == null)
					value = String.Empty;

				_base = value;
			}
		}

		/// <summary>
		/// Gets the rules.
		/// </summary>
		/// <value>The rules.</value>
		public IList<IRule> Rules
		{
			get { return _rules; }
		}
	}
}
