namespace ManagedFusion.Rewriter.Rules.Flags
{
	public class LastFlag : IRuleFlag
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LastFlag"/> class.
		/// </summary>
		public LastFlag()
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
			return RuleFlagProcessorResponse.LastRule;
		}

		#endregion
	}
}
