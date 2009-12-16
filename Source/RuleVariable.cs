using System;
using System.Collections.Generic;
using System.Text;

namespace ManagedFusion.Rewriter
{
	public class RuleVariable
	{
		private int _index;

		public RuleVariable(int index)
		{
			_index = index;
		}

		/// <summary>
		/// Gets the index.
		/// </summary>
		public int Index
		{
			get { return _index; }
		}

		/// <summary>
		/// Get the value of the rule variable.
		/// </summary>
		/// <param name="input">The input to get the variable of.</param>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public string GetValue(string input, RuleContext context)
		{
			return context.CurrentRule.Action.Pattern.GetValue(input, Index, context);
		}
	}
}
