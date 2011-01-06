using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ManagedFusion.Rewriter.Test.Rules.Flags
{
	/// <summary>
	/// Summary description for NoCaseTest
	/// </summary>
	[TestFixture]
	public class NoCaseTest : BaseTest
	{
		[Test]
		public void NoCase_Enabled()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = HttpHelpers.MockHttpContext(url);
			var target = CreateRuleSet(@"
RewriteRule ^/TEsT.aSPx$ /pass [NC]
RewriteRule ^/test.aspx$ /fail []");

			Uri expected = new Uri("http://www.somesite.com/pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void NoCase_Disabled()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = HttpHelpers.MockHttpContext(url);
			var target = CreateRuleSet(@"
RewriteRule ^/TEsT.aSPx$ /fail []
RewriteRule ^/test.aspx$ /pass []");

			Uri expected = new Uri("http://www.somesite.com/pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}
	}
}
