using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManagedFusion.Rewriter.Test
{
	[TestClass]
	public class ApacheStyleTest : BaseTest
	{
		[TestMethod]
		public void SimpleRule()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url);
			var target = CreateRuleSet(@"RewriteRule ^/([a-z]+)\.aspx  /$1 []");

			Uri expected = new Uri("http://www.somesite.com/test");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void SimpleRule_WithCondition()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url).SetServerVariables(new Dictionary<string, string> { 
				{ "REQUEST_URI", url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped) } 
			});
			var target = CreateRuleSet(@"
RewriteCond %{REQUEST_URI} ^/test\.aspx.*$ [NC]
RewriteRule ^(.*)$ /pass []");

			Uri expected = new Uri("http://www.somesite.com/pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void RewriteCondWithServerVariable()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url).SetServerVariables(new Dictionary<string, string> { 
				{ "REQUEST_URI", url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped) } 
			});
			var target = CreateRuleSet(@"
RewriteCond %{REQUEST_URI} ^/test\.aspx.*$ [NC]
RewriteRule ^/([a-z]+)\.aspx  /$1 []");

			Uri expected = new Uri("http://www.somesite.com/test");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void RewriteModule_IRuleAction()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url);
			var target = CreateRuleSet(@"
RewriteModule PostQueryString  ManagedFusion.Rewriter.Test.TestRuleAction,  ManagedFusion.Rewriter.Test
RewriteRule(,PostQueryString) ^(.*)$  $1");

			Uri expected = new Uri("http://www.somesite.com/pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void RewriteCondWithHttpHeader()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url).SetHeader(new Dictionary<string, string> { 
				{ "Accept", "text/plain" } 
			});
			var target = CreateRuleSet(@"
RewriteCond %{HTTP:Accept} ^text.*$ [NC]
RewriteRule ^/([a-z]+)\.aspx  /$1.txt []");

			Uri expected = new Uri("http://www.somesite.com/test.txt");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[TestMethod]
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
			var context = CreateHttpContext(url).SetServerVariables(new Dictionary<string, string> { 
				{ "REQUEST_URI", url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped) } 
			});

			Uri expected = new Uri("http://www.somesite.com/test");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);

			url = new Uri("http://www.somesite.com/pass.aspx");
			context = CreateHttpContext(url).SetServerVariables(new Dictionary<string, string> { 
				{ "REQUEST_URI", url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped) } 
			});

			expected = new Uri("http://www.somesite.com/pass");
			result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);

			url = new Uri("http://www.somesite.com/nick.aspx");
			context = CreateHttpContext(url).SetServerVariables(new Dictionary<string, string> { 
				{ "REQUEST_URI", url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped) } 
			});

			expected = new Uri("http://www.somesite.com/nick");
			result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);

			url = new Uri("http://www.somesite.com/fail.aspx");
			context = CreateHttpContext(url).SetServerVariables(new Dictionary<string, string> { 
				{ "REQUEST_URI", url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped) } 
			});

			expected = new Uri("http://www.somesite.com/fail");
			result = target.RunRules(context, url);

			Assert.AreNotEqual(expected, result);

			url = new Uri("http://www.somesite.com/1234.aspx");
			context = CreateHttpContext(url).SetServerVariables(new Dictionary<string, string> { 
				{ "REQUEST_URI", url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped) } 
			});

			expected = new Uri("http://www.somesite.com/1234");
			result = target.RunRules(context, url);

			Assert.AreNotEqual(expected, result);
		}
	}
}
