using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;

namespace ManagedFusion.Rewriter.Test
{
	public static class MockExtensions
	{
		public static ConditionContext CreateConditionContext(this RuleContext ruleContext)
		{
			// create properties
			var condition = new Mock<ICondition>().Object;

			return new ConditionContext(0, ruleContext, condition);
		}
	}
}
