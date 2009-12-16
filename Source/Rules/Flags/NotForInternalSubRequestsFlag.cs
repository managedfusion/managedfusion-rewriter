namespace ManagedFusion.Rewriter.Rules.Flags
{
	public class NotForInternalSubRequestsFlag : IRuleFlag
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NotForInternalSubRequestsFlag"/> class.
		/// </summary>
		public NotForInternalSubRequestsFlag()
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
