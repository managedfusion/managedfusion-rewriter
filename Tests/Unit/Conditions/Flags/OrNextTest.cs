using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace ManagedFusion.Rewriter.Test.Conditions.Flags
{
	/// <summary>
	/// Summary description for OrNextTest
	/// </summary>
	[TestFixture]
	public class OrNextTest : BaseTest
	{
		[Test]
		public void OrNext_FirstMatched()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url).SetServerVariables(new Dictionary<string, string> {
				{"REQUEST_URI", url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped)}
			});
			var target = CreateRuleSet(@"
RewriteCond %{REQUEST_URI} ^/test\.aspx.*$ [OR]
RewriteCond %{REQUEST_URI} ^/fail\.aspx.*$ [OR]
RewriteCond %{REQUEST_URI} ^/fail1\.aspx.*$ [OR]
RewriteCond %{REQUEST_URI} ^/fail2\.aspx.*$ []
RewriteRule ^/test.aspx$  /pass []
RewriteRule ^/test.aspx$  /fail []");

			Uri expected = new Uri("http://www.somesite.com/pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void OrNext_MiddleMatched()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url).SetServerVariables(new Dictionary<string, string> {
				{"REQUEST_URI", url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped)}
			});
			var target = CreateRuleSet(@"
RewriteCond %{REQUEST_URI} ^/fail\.aspx.*$ [OR]
RewriteCond %{REQUEST_URI} ^/test\.aspx.*$ [OR]
RewriteCond %{REQUEST_URI} ^/fail1\.aspx.*$ [OR]
RewriteCond %{REQUEST_URI} ^/fail2\.aspx.*$ []
RewriteRule ^/test.aspx$  /pass []
RewriteRule ^/test.aspx$  /fail []");

			Uri expected = new Uri("http://www.somesite.com/pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void OrNext_LastMatched()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url).SetServerVariables(new Dictionary<string, string> {
				{"REQUEST_URI", url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped)}
			});
			var target = CreateRuleSet(@"
RewriteCond %{REQUEST_URI} ^/fail\.aspx.*$ [OR]
RewriteCond %{REQUEST_URI} ^/fail1\.aspx.*$ [OR]
RewriteCond %{REQUEST_URI} ^/test\.aspx.*$ [OR]
RewriteCond %{REQUEST_URI} ^/fail2\.aspx.*$ []
RewriteRule ^/test.aspx$  /pass []
RewriteRule ^/test.aspx$  /fail []");

			Uri expected = new Uri("http://www.somesite.com/pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void OrNext_FinalMatched()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url).SetServerVariables(new Dictionary<string, string> {
				{"REQUEST_URI", url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped)}
			});
			var target = CreateRuleSet(@"
RewriteCond %{REQUEST_URI} ^/fail\.aspx.*$ [OR]
RewriteCond %{REQUEST_URI} ^/fail1\.aspx.*$ [OR]
RewriteCond %{REQUEST_URI} ^/fail2\.aspx.*$ [OR]
RewriteCond %{REQUEST_URI} ^/test\.aspx.*$ []
RewriteRule ^/test.aspx$  /pass []
RewriteRule ^/test.aspx$  /fail []");

			Uri expected = new Uri("http://www.somesite.com/pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void OrNext_NotlMatched()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url).SetServerVariables(new Dictionary<string, string> {
				{"REQUEST_URI", url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped)}
			});
			var target = CreateRuleSet(@"
RewriteCond %{REQUEST_URI} ^/fail\.aspx.*$ [OR]
RewriteCond %{REQUEST_URI} ^/fail1\.aspx.*$ [OR]
RewriteCond %{REQUEST_URI} ^/fail2\.aspx.*$ [OR]
RewriteCond %{REQUEST_URI} ^/fail3\.aspx.*$ []
RewriteRule ^/test.aspx$  /fail []
RewriteRule ^/test.aspx$  /pass []");

			Uri expected = new Uri("http://www.somesite.com/pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}
	}
}
