using System;
using NUnit.Framework;

namespace ManagedFusion.Rewriter.Test.Rules.Flags
{
	[TestFixture]
	public class SkipRuleFlagTest : BaseTest
	{
		[Test]
		public void Skip()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url);
			var target = CreateRuleSet(@"
RewriteRule ^(.*)$ $1 [S=3]
RewriteRule ^(.*)$ /fail []
RewriteRule ^(.*)$ /fail []
RewriteRule ^(.*)$ /fail []
RewriteRule ^/test.aspx$ /pass []");

			Uri expected = new Uri("http://www.somesite.com/pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}
	}
}
