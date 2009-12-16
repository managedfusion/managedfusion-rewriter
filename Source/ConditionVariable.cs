using System;
using System.Collections.Generic;
using System.Text;

namespace ManagedFusion.Rewriter
{
	public class ConditionVariable
	{
		private int _index;

		public ConditionVariable(int index)
		{
			_index = index;
		}

		public int Index
		{
			get { return _index; }
		}

		public string GetValue(string input, RuleContext context)
		{
			int startIndex = 1;

			for (int i = 0; i < context.Conditions.Count; i++)
			{
				var cond = context.Conditions[i];
				int groupCount = cond.Pattern.GetGroupCount(input);
				ConditionContext condContext = new ConditionContext(i, context, cond);

				if ((startIndex + groupCount) >= Index)
				{
					int varIndex = Math.Abs(startIndex - _index);
					return cond.Pattern.GetValue(cond.Test.GetValue(condContext), varIndex, condContext);
				}

				startIndex += groupCount;
			}

			return null;
		}
	}
}
