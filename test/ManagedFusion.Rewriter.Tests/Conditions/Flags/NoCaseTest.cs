using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace ManagedFusion.Rewriter.Tests.Conditions.Flags
{
	/// <summary>
	/// Summary description for NoCaseTest
	/// </summary>
	[TestFixture]
	public class NoCaseTest : BaseTest
	{
		[Test]
		public void NoCase_Enabled()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = HttpHelpers.MockHttpContext(url);
			context.Request.SetServerVariables(new Dictionary<string, string> { 
				{ "HTTP_HOST", url.GetComponents(UriComponents.Host, UriFormat.SafeUnescaped) } 
			});
			var target = CreateRuleSet(@"
RewriteCond %{HTTP_HOST} ^(WwW).*$ [NC]
RewriteRule ^/test.aspx$ /pass []
RewriteRule ^/test.aspx$ /fail []");

			Uri expected = new Uri("http://www.somesite.com/pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void NoCase_Disabled()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = HttpHelpers.MockHttpContext(url);
			context.Request.SetServerVariables(new Dictionary<string, string> { 
				{ "HTTP_HOST", url.GetComponents(UriComponents.Host, UriFormat.SafeUnescaped) } 
			});
			var target = CreateRuleSet(@"
RewriteCond %{HTTP_HOST} ^(WwW).*$ []
RewriteRule ^/test.aspx$ /fail []
RewriteRule ^/test.aspx$ /pass []");

			Uri expected = new Uri("http://www.somesite.com/pass");
			Uri result = target.RunRules(context, url);

			Assert.AreEqual(expected, result);
		}
	}
}
