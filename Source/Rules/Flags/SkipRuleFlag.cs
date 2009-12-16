using System;


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
