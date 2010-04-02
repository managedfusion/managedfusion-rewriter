using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManagedFusion.Rewriter.Test.Rules.Flags
{
	/// <summary>
	/// Summary description for LastFlagTest
	/// </summary>
	[TestClass]
	public class LastFlagTest : BaseTest
	{
		[TestMethod]
		public void Last_First()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url);
			var target = CreateRuleSet(@"
RewriteRule ^(.*)$ /pass [L]
RewriteRule ^(.*)$ /fail [L]
RewriteRule ^(.*)$ /fail [L]");

			Uri expected = new Uri("http://www.somesite.com/pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void Last_Middle()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url);
			var target = CreateRuleSet(@"
RewriteRule ^(.*)$ /fail
RewriteRule ^(.*)$ /pass [L]
RewriteRule ^(.*)$ /fail [L]");

			Uri expected = new Uri("http://www.somesite.com/pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void Last_Final()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url);
			var target = CreateRuleSet(@"
RewriteRule ^(.*)$ /fail
RewriteRule ^(.*)$ /fail
RewriteRule ^(.*)$ /pass [L]");

			Uri expected = new Uri("http://www.somesite.com/pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}
	}
}
