using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace ManagedFusion.Rewriter.Test
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
			var context = CreateHttpContext(url).SetServerVariables(new Dictionary<string, string> {
				{ "QUERY_STRING", "id=%{HTTP_HOST}" },
				{ "HTTP_HOST", "somesite.com" }
			});

			Uri expected = new Uri("http://somesite.com/pass/%{HTTP_HOST}/");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}
	}
}
