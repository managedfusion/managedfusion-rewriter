namespace ManagedFusion.Rewriter.Rules.Flags
{
	public class NoCaseFlag : IRuleFlag
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NoCaseFlag"/> class.
		/// </summary>
		public NoCaseFlag()
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
			return RuleFlagProcessorResponse.ContinueToNextFlag;
		}

		#endregion
	}
}
