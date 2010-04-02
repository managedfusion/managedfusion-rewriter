using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManagedFusion.Rewriter.Test.Rules.Flags
{
	[TestClass]
	public class QueryStringAppendFlagTest : BaseTest
	{
		[TestMethod]
		public void QueryStringAppend_WithQueryString()
		{
			var url = new Uri("http://www.somesite.com/test.aspx?type2=pass");
			var context = CreateHttpContext(url);
			var target = CreateRuleSet(@"RewriteRule ^(.*)$ /test?type=pass [QSA]");

			Uri expected = new Uri("http://www.somesite.com/test?type=pass&type2=pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void QueryStringAppend_WithOutQueryString()
		{
			var url = new Uri("http://www.somesite.com/test.aspx?type2=pass");
			var context = CreateHttpContext(url);
			var target = CreateRuleSet(@"RewriteRule ^(.*)$ /test [QSA]");

			Uri expected = new Uri("http://www.somesite.com/test?type2=pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void QueryStringAppend_WithQueryString_NoQueryStringOnUrl()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url);
			var target = CreateRuleSet(@"RewriteRule ^(.*)$ /test?type=pass [QSA]");

			Uri expected = new Uri("http://www.somesite.com/test?type=pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void QueryStringAppend_WithOutQueryString_NoQueryStringOnUrl()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url);
			var target = CreateRuleSet(@"RewriteRule ^(.*)$ /test [QSA]");

			Uri expected = new Uri("http://www.somesite.com/test");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}
	}
}
