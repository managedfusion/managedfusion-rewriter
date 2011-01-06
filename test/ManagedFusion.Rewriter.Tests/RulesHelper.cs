using System;
using Moq;

namespace ManagedFusion.Rewriter.Test
{
	public static class RulesHelper
	{
		public static ConditionContext CreateConditionContext(this RuleContext ruleContext)
		{
			// create properties
			var condition = new Mock<ICondition>().Object;

			return new ConditionContext(0, ruleContext, condition);
		}
	}
}