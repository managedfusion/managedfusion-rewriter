using System;

namespace ManagedFusion.Rewriter.Test
{
	public class TestRuleAction : IRuleAction
	{
		#region IRuleSubstitution Members

		public Pattern Pattern
		{
			get;
			private set;
		}

		public string Substitution
		{
			get;
			private set;
		}

		public void Execute(RuleContext context)
		{
			context.SubstitutedUrl = new Uri("http://www.somesite.com/pass");
		}

		public bool IsMatch(RuleContext context)
		{
			return true;
		}

		public void Init(Pattern pattern, string text)
		{
			Pattern = pattern;
			Substitution = text;
		}

		#endregion
	}
}
