using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace ManagedFusion.Rewriter.Tests
{
	[TestFixture]
	public class CommonProblemTest : BaseTest
	{
		[Test]
		public void VariableDoubleProcessingNotAllowed()
		{
			var target = CreateRuleSet(@"
RewriteCond %{QUERY_STRING} ^id=(.*)$
RewriteRule ^/test.aspx$ /pass/%2/ [L]");

			var url = new Uri("http://somesite.com/test.aspx");
			var context = HttpHelpers.MockHttpContext(url);
			context.Request.SetServerVariables(new Dictionary<string, string> {
				{ "QUERY_STRING", "id=%{HTTP_HOST}" },
				{ "HTTP_HOST", "somesite.com" }
			});

			Uri expected = new Uri("http://somesite.com/pass/%{HTTP_HOST}/");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void AddQueryStringParameterOnToUrlThatContainsQueryString()
		{
			var target = CreateRuleSet(@"
RewriteRule ^/(.*)$  /$1?test=added [QSA]");

			var url = new Uri("http://somesite.com/pass?test=1&test=2&test=3");
			var context = HttpHelpers.MockHttpContext(url);
			context.Request.SetServerVariables(new Dictionary<string,string> {
				{ "QUERY_STRING", "test=1&test=2&test=3" },
				{ "HTTP_HOST", "somesite.com" }
			});

			Uri expected = new Uri("http://somesite.com/pass?test=added&test=1&test=2&test=3");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}
	}
}