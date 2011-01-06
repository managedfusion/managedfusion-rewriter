using System;
using NUnit.Framework;

namespace ManagedFusion.Rewriter.Test.Rules.Flags
{
	[TestFixture]
	public class QueryStringAppendFlagTest : BaseTest
	{
		[Test]
		public void QueryStringAppend_WithQueryString()
		{
			var url = new Uri("http://www.somesite.com/test.aspx?type2=pass");
			var context = HttpHelpers.MockHttpContext(url);
			var target = CreateRuleSet(@"RewriteRule ^(.*)$ /test?type=pass [QSA]");

			Uri expected = new Uri("http://www.somesite.com/test?type=pass&type2=pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void QueryStringAppend_WithOutQueryString()
		{
			var url = new Uri("http://www.somesite.com/test.aspx?type2=pass");
			var context = HttpHelpers.MockHttpContext(url);
			var target = CreateRuleSet(@"RewriteRule ^(.*)$ /test [QSA]");

			Uri expected = new Uri("http://www.somesite.com/test?type2=pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void QueryStringAppend_WithQueryString_NoQueryStringOnUrl()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = HttpHelpers.MockHttpContext(url);
			var target = CreateRuleSet(@"RewriteRule ^(.*)$ /test?type=pass [QSA]");

			Uri expected = new Uri("http://www.somesite.com/test?type=pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void QueryStringAppend_WithOutQueryString_NoQueryStringOnUrl()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = HttpHelpers.MockHttpContext(url);
			var target = CreateRuleSet(@"RewriteRule ^(.*)$ /test [QSA]");

			Uri expected = new Uri("http://www.somesite.com/test");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}
	}
}
