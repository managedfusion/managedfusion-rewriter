using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace ManagedFusion.Rewriter.Test
{
	[TestClass]
	public class PatternTest : BaseTest
	{
		[TestMethod]
		public void InvertMatch_True()
		{
			string pattern = "!^(www).*$";
			RegexOptions options = Manager.RuleOptions;
			Pattern target = new Pattern(pattern, options);

			bool actual = target.InvertMatch;

			Assert.AreEqual(true, actual);
		}

		[TestMethod]
		public void InvertMatch_False()
		{
			string pattern = "^(www).*$";
			RegexOptions options = Manager.RuleOptions;
			Pattern target = new Pattern(pattern, options);

			bool actual = target.InvertMatch;

			Assert.AreEqual(false, actual);
		}

		[TestMethod]
		public void Replace_ServerVariable()
		{
			var url = new Uri("http://www.somesite.com/test.aspx");
			var httpContext = CreateHttpContext(url).SetServerVariables(new Dictionary<string, string> { 
				{ "SERVER_PORT", "1234" } 
			});
			var context = CreateRuleContext(url, httpContext);
			string pattern = "^(.*)$";
			string input = "/test.aspx";
			string replacement = "/pass?port=%{SERVER_PORT}";
			RegexOptions options = Manager.RuleOptions;
			Pattern target = new Pattern(pattern, options);

			string result = Pattern.Replace(replacement, context);
			string expected = "/pass?port=1234";

			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public new void ToString()
		{
			string pattern = "!^(www).*$";
			RegexOptions options = Manager.RuleOptions;
			Pattern target = new Pattern(pattern, options);

			string expected = pattern;
			string actual = target.ToString();

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ToString_True()
		{
			string pattern = "!^(www).*$";
			RegexOptions options = Manager.RuleOptions;
			Pattern target = new Pattern(pattern, options);

			bool showOnlyPattern = true;
			string expected = "^(www).*$";
			string actual = target.ToString(showOnlyPattern);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ToString_False()
		{
			string pattern = "!^(www).*$";
			RegexOptions options = Manager.RuleOptions;
			Pattern target = new Pattern(pattern, options);

			bool showOnlyPattern = false;
			string expected = pattern;
			string actual = target.ToString(showOnlyPattern);

			Assert.AreEqual(expected, actual);
		}
	}
}
