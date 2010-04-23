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

using System.Collections.Generic;

namespace ManagedFusion.Rewriter
{
	/// <summary>
	/// 
	/// </summary>
	public interface IRule
	{
		/// <summary>
		/// Gets the name.
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Gets the conditions.
		/// </summary>
		/// <value>The conditions.</value>
		IList<ICondition> Conditions { get; }

		/// <summary>
		/// Gets the action.
		/// </summary>
		/// <value>The action.</value>
		IRuleAction Action { get; }

		/// <summary>
		/// Gets the flags.
		/// </summary>
		/// <value>The flags.</value>
		IRuleFlagProcessor Flags { get; }

		/// <summary>
		/// Tries the process.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		bool TryExecute(RuleContext context);

		/// <summary>
		/// Inits the specified conditions.
		/// </summary>
		/// <param name="conditions">The conditions.</param>
		/// <param name="action">The action.</param>
		/// <param name="flags">The flags.</param>
		void Init(IEnumerable<ICondition> conditions, IRuleAction action, IRuleFlagProcessor flags);
	}
}
