using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManagedFusion.Rewriter.Test.Rules.Flags
{
	/// <summary>
	/// Summary description for ChainFlagTest
	/// </summary>
	[TestClass]
	public class ChainFlagTest : BaseTest
	{
		[TestMethod]
		public void Chain_FirstNotMatched()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url);
			var target = CreateRuleSet(@"
RewriteRule ^/fail.aspx$ /fail?test=1 [C]
RewriteRule ^(.*)$ /fail?test=2 [C]
RewriteRule ^(.*)$ /fail?test=3 [C]
RewriteRule ^(.*)$ /fail?test=4 []
RewriteRule ^/test.aspx$ /pass []");

			Uri expected = new Uri("http://www.somesite.com/pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void Chain_MiddleNotMatched()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url);
			var target = CreateRuleSet(@"
RewriteRule ^(.*)$ /fail?test=1 [C]
RewriteRule ^/fail.aspx$ /fail?test=2 [C]
RewriteRule ^(.*)$ /fail?test=3 [C]
RewriteRule ^(.*)$ /fail?test=4 []
RewriteRule ^/test.aspx$ /pass []");

			Uri expected = new Uri("http://www.somesite.com/pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void Chain_LastNotMatched()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url);
			var target = CreateRuleSet(@"
RewriteRule ^(.*)$ /fail?test=1 [C]
RewriteRule ^(.*)$ /fail?test=2 [C]
RewriteRule ^/fail.aspx$ /fail?test=3 [C]
RewriteRule ^(.*)$ /fail?test=4 []
RewriteRule ^/test.aspx$ /pass []");

			Uri expected = new Uri("http://www.somesite.com/pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void Chain_FinalNotMatched()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url);
			var target = CreateRuleSet(@"
RewriteRule ^(.*)$ /fail?test=1 [C]
RewriteRule ^(.*)$ /fail?test=2 [C]
RewriteRule ^(.*)$ /fail?test=3 [C]
RewriteRule ^/fail.aspx$ /fail?test=4 []
RewriteRule ^/test.aspx$ /pass []");

			Uri expected = new Uri("http://www.somesite.com/pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void Chain_Matched()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url);
			var target = CreateRuleSet(@"
RewriteRule ^/test.aspx$ /pass/test1 [C]
RewriteRule ^/pass(.*)$ /pass/test2$1 [C]
RewriteRule ^/pass(.*)$ /pass/test3$1 [C]
RewriteRule ^/pass(.*)$ /pass/test4$1 []
RewriteRule ^/test.aspx$ /fail []");

			Uri expected = new Uri("http://www.somesite.com/pass/test4/test3/test2/test1");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}
	}
}
