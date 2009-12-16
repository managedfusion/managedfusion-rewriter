namespace ManagedFusion.Rewriter.Rules.Flags
{
	public class ChainFlag : IRuleFlag
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ChainFlag"/> class.
		/// </summary>
		public ChainFlag()
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
			return RuleFlagProcessorResponse.ContinueToNextRule;
		}

		#endregion
	}
}
