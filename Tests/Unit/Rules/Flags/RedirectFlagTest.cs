using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManagedFusion.Rewriter.Test.Rules.Flags
{
	/// <summary>
	/// Summary description for ChainFlagTest
	/// </summary>
	[TestClass]
	public class RedirectFlagTest : BaseTest
	{
		[TestMethod]
		public void Redirect_Plain()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url);
			var target = CreateRuleSet(@"
RewriteRule ^/test.aspx$   /pass   [R]");

			int expectedStatus = 302;
			string expected = "http://www.somesite.com/pass";

			Uri resultUrl = target.RunRules(context, url);

			int resultStatus = context.Response.StatusCode;
			string result = context.Response.RedirectLocation;

			Assert.AreEqual(expectedStatus, resultStatus);
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void Redirect_301()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url);
			var target = CreateRuleSet(@"
RewriteRule ^/test.aspx$   /pass   [R=301]");

			int expectedStatus = 301;
			string expected = "http://www.somesite.com/pass";

			Uri resultUrl = target.RunRules(context, url);

			int resultStatus = context.Response.StatusCode;
			string result = context.Response.RedirectLocation;

			Assert.AreEqual(expectedStatus, resultStatus);
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void Redirect_302()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url);
			var target = CreateRuleSet(@"
RewriteRule ^/test.aspx$   /pass   [R=302]");

			int expectedStatus = 302;
			string expected = "http://www.somesite.com/pass";

			Uri resultUrl = target.RunRules(context, url);

			int resultStatus = context.Response.StatusCode;
			string result = context.Response.RedirectLocation;

			Assert.AreEqual(expectedStatus, resultStatus);
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void Redirect_303()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url);
			var target = CreateRuleSet(@"
RewriteRule ^/test.aspx$   /pass   [R=303]");

			int expectedStatus = 303;
			string expected = "http://www.somesite.com/pass";

			Uri resultUrl = target.RunRules(context, url);

			int resultStatus = context.Response.StatusCode;
			string result = context.Response.RedirectLocation;

			Assert.AreEqual(expectedStatus, resultStatus);
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void Redirect_307()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url);
			var target = CreateRuleSet(@"
RewriteRule ^/test.aspx$   /pass   [R=307]");

			int expectedStatus = 307;
			string expected = "http://www.somesite.com/pass";

			Uri resultUrl = target.RunRules(context, url);

			int resultStatus = context.Response.StatusCode;
			string result = context.Response.RedirectLocation;

			Assert.AreEqual(expectedStatus, resultStatus);
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void Redirect_Permanent()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url);
			var target = CreateRuleSet(@"
RewriteRule ^/test.aspx$   /pass   [R=permanent]");

			int expectedStatus = 301;
			string expected = "http://www.somesite.com/pass";

			Uri resultUrl = target.RunRules(context, url);

			int resultStatus = context.Response.StatusCode;
			string result = context.Response.RedirectLocation;

			Assert.AreEqual(expectedStatus, resultStatus);
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void Redirect_Found()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url);
			var target = CreateRuleSet(@"
RewriteRule ^/test.aspx$   /pass   [R=found]");

			int expectedStatus = 302;
			string expected = "http://www.somesite.com/pass";

			Uri resultUrl = target.RunRules(context, url);

			int resultStatus = context.Response.StatusCode;
			string result = context.Response.RedirectLocation;

			Assert.AreEqual(expectedStatus, resultStatus);
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void Redirect_SeeOther()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url);
			var target = CreateRuleSet(@"
RewriteRule ^/test.aspx$   /pass   [R=seeother]");

			int expectedStatus = 303;
			string expected = "http://www.somesite.com/pass";

			Uri resultUrl = target.RunRules(context, url);

			int resultStatus = context.Response.StatusCode;
			string result = context.Response.RedirectLocation;

			Assert.AreEqual(expectedStatus, resultStatus);
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void Redirect_Temp()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var context = CreateHttpContext(url);
			var target = CreateRuleSet(@"
RewriteRule ^/test.aspx$   /pass   [R=temp]");

			int expectedStatus = 307;
			string expected = "http://www.somesite.com/pass";

			Uri resultUrl = target.RunRules(context, url);

			int resultStatus = context.Response.StatusCode;
			string result = context.Response.RedirectLocation;

			Assert.AreEqual(expectedStatus, resultStatus);
			Assert.AreEqual(expected, result);
		}
	}
}
