

namespace ManagedFusion.Rewriter.Rules.Flags
{
	public class NextRuleFlag : IRuleFlag
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NextRuleFlag"/> class.
		/// </summary>
		public NextRuleFlag()
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
			context.RuleIndex = -1;
			Manager.LogIf(context.LogLevel >= 2, "Restart Rule Processing", "Rewrite");

			return RuleFlagProcessorResponse.ContinueToNextRule;
		}

		#endregion
	}
}
