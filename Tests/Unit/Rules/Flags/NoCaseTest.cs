using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManagedFusion.Rewriter.Test.Rules.Flags
{
	/// <summary>
	/// Summary description for NoCaseTest
	/// </summary>
	[TestClass]
	public class NoCaseTest : BaseTest
	{
		[TestMethod]
		public void NoCase_Enabled()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url);
			var target = CreateRuleSet(@"
RewriteRule ^/TEsT.aSPx$ /pass [NC]
RewriteRule ^/test.aspx$ /fail []");

			Uri expected = new Uri("http://www.somesite.com/pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void NoCase_Disabled()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url);
			var target = CreateRuleSet(@"
RewriteRule ^/TEsT.aSPx$ /fail []
RewriteRule ^/test.aspx$ /pass []");

			Uri expected = new Uri("http://www.somesite.com/pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}
	}
}
