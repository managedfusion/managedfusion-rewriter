using System;
using NUnit.Framework;

namespace ManagedFusion.Rewriter.Tests.Rules.Flags
{
	/// <summary>
	/// Summary description for NextRuleFlagTest
	/// </summary>
	[TestFixture]
	public class NextRuleFlagTest : BaseTest
	{
		[Test]
		public void Next()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = HttpHelpers.MockHttpContext(url);
			var target = CreateRuleSet(@"
RewriteRule ^/fail$ /pass []
RewriteRule ^/test.aspx$ /fail [N]");

			Uri expected = new Uri("http://www.somesite.com/pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}
	}
}
