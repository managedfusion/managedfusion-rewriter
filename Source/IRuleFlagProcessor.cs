using System.Collections.Generic;

namespace ManagedFusion.Rewriter
{
	public interface IRuleFlagProcessor : IEnumerable<IRuleFlag>
	{
		/// <summary>
		/// Adds the specified flag.
		/// </summary>
		/// <param name="flag">The flag.</param>
		void Add(IRuleFlag flag);

		/// <summary>
		/// Applies the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		RuleFlagProcessorResponse Apply(RuleContext context);
	}
}
