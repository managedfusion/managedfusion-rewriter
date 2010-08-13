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

namespace ManagedFusion.Rewriter.Rules.Flags
{
	/// <summary>
	/// This flag forces the rewriting engine to skip the next num rules in sequence, if the current rule matches. Use this to make pseudo if-then-else constructs: The last rule of the then-clause becomes skip=N, where N is the number of rules in the else-clause. (This is not the same as the 'chain|C' flag!)
	/// </summary>
	public class SkipRuleFlag : IRuleFlag
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SkipRuleFlag"/> class.
		/// </summary>
		/// <param name="index">The index.</param>
		public SkipRuleFlag(int count)
		{
			Count = count;
		}

		/// <summary>
		/// Gets or sets the count.
		/// </summary>
		/// <value>The count.</value>
		public int Count { get; private set; }

		#region IRuleFlag Members

		/// <summary>
		/// Applies the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public RuleFlagProcessorResponse Apply(RuleContext context)
		{
			// skip to a certain rule or if the value is not set just skip to the next rule
			context.RuleIndex = (context.RuleIndex ?? -1) + Count;
			Manager.LogIf(context.LogLevel >= 2, "Skip The Next + " + Count + " To Rule " + context.RuleIndex, "Rewrite");

			return RuleFlagProcessorResponse.ContinueToNextRule;
		}

		#endregion
	}
}
