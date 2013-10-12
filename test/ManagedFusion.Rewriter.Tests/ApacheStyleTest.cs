using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace ManagedFusion.Rewriter.Tests
{
	[TestFixture]
	public class ApacheStyleTest : BaseTest
	{
		[Test]
		public void SimpleRule()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = HttpHelpers.MockHttpContext(url);
			var target = CreateRuleSet(@"RewriteRule ^/([a-z]+)\.aspx  /$1 []");

			Uri expected = new Uri("http://www.somesite.com/test");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void SimpleRule_WithCondition()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = HttpHelpers.MockHttpContext(url);
			context.Request.SetServerVariables(new Dictionary<string, string> { 
				{ "REQUEST_URI", url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped) } 
			});
			var target = CreateRuleSet(@"
RewriteCond %{REQUEST_URI} ^/test\.aspx.*$ [NC]
RewriteRule ^(.*)$ /pass []");

			Uri expected = new Uri("http://www.somesite.com/pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void RewriteCondWithServerVariable()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = HttpHelpers.MockHttpContext(url);
			context.Request.SetServerVariables(new Dictionary<string, string> { 
				{ "REQUEST_URI", url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped) } 
			});
			var target = CreateRuleSet(@"
RewriteCond %{REQUEST_URI} ^/test\.aspx.*$ [NC]
RewriteRule ^/([a-z]+)\.aspx  /$1 []");

			Uri expected = new Uri("http://www.somesite.com/test");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void RewriteModule_IRuleAction()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = HttpHelpers.MockHttpContext(url);
			var target = CreateRuleSet(@"
RewriteModule PostQueryString  ManagedFusion.Rewriter.Tests.TestRuleAction,  ManagedFusion.Rewriter.Tests
RewriteRule(,PostQueryString) ^(.*)$  $1");

			Uri expected = new Uri("http://www.somesite.com/pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void RewriteCondWithHttpHeader()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = HttpHelpers.MockHttpContext(url);
			context.Request.SetHeaders(new Dictionary<string, string> { 
				{ "Accept", "text/plain" } 
			});
			var target = CreateRuleSet(@"
RewriteCond %{HTTP:Accept} ^text.*$ [NC]
RewriteRule ^/([a-z]+)\.aspx  /$1.txt []");

			Uri expected = new Uri("http://www.somesite.com/test.txt");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void SimpleRule_WithNoSubstituion()
		{
			var target = CreateRuleSet(@"
RewriteRule ^.* - [F,L]");

			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = HttpHelpers.MockHttpContext(url);
			context.Request.SetServerVariables(new Dictionary<string, string> { 
				{ "REQUEST_URI", url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped) } 
			});

			Uri expected = null; // no change
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void SimpleRule_WithComplexCondition()
		{
			var target = CreateRuleSet(@"
RewriteCond %{REQUEST_URI} ^/[a-z]+\.aspx.*$ [NC]
RewriteCond %{REQUEST_URI} ^/test\.aspx.*$ [NC, OR]
RewriteCond %{REQUEST_URI} ^/pass\.aspx.*$ [NC, OR]
RewriteCond %{REQUEST_URI} ^/[a-z]{4,4}\.aspx.*$ [NC]
RewriteCond %{REQUEST_URI} !^/fail\.aspx.*$ [NC]
RewriteRule ^/([a-z]+)\.aspx  /$1 []");

			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = HttpHelpers.MockHttpContext(url);
			context.Request.SetServerVariables(new Dictionary<string, string> { 
				{ "REQUEST_URI", url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped) } 
			});

			Uri expected = new Uri("http://www.somesite.com/test");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);

			url = new Uri("http://www.somesite.com/pass.aspx");
			context = HttpHelpers.MockHttpContext(url);
			context.Request.SetServerVariables(new Dictionary<string, string> { 
				{ "REQUEST_URI", url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped) } 
			});

			expected = new Uri("http://www.somesite.com/pass");
			result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);

			url = new Uri("http://www.somesite.com/nick.aspx");
			context = HttpHelpers.MockHttpContext(url);
			context.Request.SetServerVariables(new Dictionary<string, string> { 
				{ "REQUEST_URI", url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped) } 
			});

			expected = new Uri("http://www.somesite.com/nick");
			result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);

			url = new Uri("http://www.somesite.com/fail.aspx");
			context = HttpHelpers.MockHttpContext(url);
			context.Request.SetServerVariables(new Dictionary<string, string> { 
				{ "REQUEST_URI", url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped) } 
			});

			expected = new Uri("http://www.somesite.com/fail");
			result = target.RunRules(context, url);

			Assert.AreNotEqual(expected, result);

			url = new Uri("http://www.somesite.com/1234.aspx");
			context = HttpHelpers.MockHttpContext(url);
			context.Request.SetServerVariables(new Dictionary<string, string> { 
				{ "REQUEST_URI", url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped) } 
			});

			expected = new Uri("http://www.somesite.com/1234");
			result = target.RunRules(context, url);

			Assert.AreNotEqual(expected, result);
		}
	}
}
