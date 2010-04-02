using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManagedFusion.Rewriter.Test.Conditions.Flags
{
	/// <summary>
	/// Summary description for NoCaseTest
	/// </summary>
	[TestClass]
	public class NoCaseTest : BaseTest
	{
		[TestMethod]
		public void NoCase_Enabled()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url).SetServerVariables(new Dictionary<string, string> { 
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

		[TestMethod]
		public void NoCase_Disabled()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url).SetServerVariables(new Dictionary<string, string> { 
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
