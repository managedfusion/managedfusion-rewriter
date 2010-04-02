using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManagedFusion.Rewriter.Test.Rules.Flags
{
	/// <summary>
	/// Summary description for NextRuleFlagTest
	/// </summary>
	[TestClass]
	public class NextRuleFlagTest : BaseTest
	{
		[TestMethod]
		public void Next()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url);
			var target = CreateRuleSet(@"
RewriteRule ^/fail$ /pass []
RewriteRule ^/test.aspx$ /fail [N]");

			Uri expected = new Uri("http://www.somesite.com/pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}
	}
}
