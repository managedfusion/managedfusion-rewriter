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

namespace ManagedFusion.Rewriter
{
	/// <summary>
	/// 
	/// </summary>
	public class ConditionContext : RuleContext
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConditionContext"/> class.
		/// </summary>
		/// <param name="copy">The copy.</param>
		internal ConditionContext(ConditionContext copy)
			: base(copy)
		{
			if (copy == null)
				throw new ArgumentNullException("copy");

			ConditionIndex = copy.ConditionIndex;
			LogCategory = copy.LogCategory;
			CurrentCondition = copy.CurrentCondition;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConditionContext"/> class.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="ruleContext">The rule context.</param>
		/// <param name="condition">The condition.</param>
		public ConditionContext(int index, RuleContext ruleContext, ICondition condition)
			: base(ruleContext)
		{
			if (condition == null)
				throw new ArgumentNullException("condition");

			ConditionIndex = index;
			LogCategory = "Condition " + index;
			CurrentCondition = condition;
		}

		/// <summary>
		/// Gets or sets the index.
		/// </summary>
		/// <value>The index.</value>
		public int? ConditionIndex { get; set; }

		/// <summary>
		/// Gets or sets the current condition.
		/// </summary>
		/// <value>The current condition.</value>
		public ICondition CurrentCondition { get; private set; }

		/// <summary>
		/// Gets the flags.
		/// </summary>
		/// <value>The flags.</value>
		public virtual IConditionFlagProcessor ConditionFlags
		{
			get { return CurrentCondition.Flags; }
		}
	}
}
