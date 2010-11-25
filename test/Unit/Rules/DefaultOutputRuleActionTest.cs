using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using ManagedFusion.Rewriter.Test;
using ManagedFusion.Rewriter.Conditions;

namespace ManagedFusion.Rewriter.Rules.Test
{
	[TestFixture]
	public class DefaultOutputRuleActionTest : BaseTest
	{
		[Test]
		public void VerifyExecute()
		{
			var text = "/$1";
			var pattern = new Pattern("/([a-z]+)/index.aspx", RegexOptions.IgnoreCase | RegexOptions.Singleline);
			var target = new DefaultOutputRuleAction(pattern, text);

			var httpContext = CreateHttpContext(new Uri("http://www.somesite.com/success/index.aspx"));
			string content = CreateHtmlContent(@"<a href=""http://www.somesite.com/success/index.aspx"" target=""_blank"">Link</a>");
			var rule = MockRule(null, target, null);

			RuleContext context = CreateOutputRuleContext(content.ToByteArray(), httpContext, rule);
			target.Execute(context);
			
			string result = context.SubstitutedContent.GetString();
			string expected = CreateHtmlContent(@"<a href=""http://www.somesite.com/success"" target=""_blank"">Link</a>");

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void VerifyExecuteWithServerVariable()
		{
			var text = "http://%{HTTP_HOST}/$1";
			var pattern = new Pattern("/([a-z]+)/index.aspx", RegexOptions.IgnoreCase | RegexOptions.Singleline);
			var target = new DefaultOutputRuleAction(pattern, text);

			var httpContext = CreateHttpContext(new Uri("http://www.managedfusion.com/success/index.aspx")).SetServerVariables(new Dictionary<string, string> { 
				{ "HTTP_HOST", "www.managedfusion.com" } 
			});
			string content = CreateHtmlContent(@"<a href=""/success/index.aspx"" target=""_blank"">Link</a>");
			var rule = MockRule(null, target, null);

			RuleContext context = CreateOutputRuleContext(content.ToByteArray(), httpContext, rule);
			target.Execute(context);

			string result = context.SubstitutedContent.GetString();
			string expected = CreateHtmlContent(@"<a href=""http://www.managedfusion.com/success"" target=""_blank"">Link</a>");

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void VerifyExecuteWithOneConditionalValue()
		{
			var text = "http://%1/$1";
			var pattern = new Pattern("/([a-z]+)/index.aspx", RegexOptions.IgnoreCase | RegexOptions.Singleline);
			var target = new DefaultOutputRuleAction(pattern, text);

			var condTest = new DefaultConditionTestValue("%{HTTP_HOST}");
			var condPattern = new Pattern(".*", RegexOptions.IgnoreCase | RegexOptions.Singleline);
			var cond = MockCond(condPattern, condTest, null);

			var httpContext = CreateHttpContext(new Uri("http://www.managedfusion.com/success/index.aspx")).SetServerVariables(new Dictionary<string, string> { 
				{ "HTTP_HOST", "www.managedfusion.com" } 
			});
			string content = CreateHtmlContent(@"<a href=""/success/index.aspx"" target=""_blank"">Link</a>");
			var rule = MockRule(new List<ICondition> { cond }, target, null);

			RuleContext context = CreateOutputRuleContext(content.ToByteArray(), httpContext, rule);
			target.Execute(context);

			string result = context.SubstitutedContent.GetString();
			string expected = CreateHtmlContent(@"<a href=""http://www.managedfusion.com/success"" target=""_blank"">Link</a>");

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void VerifyExecuteWithNineConditionalValues()
		{
			var text = "http://%3/%2/%1/$1";
			var pattern = new Pattern("/([a-z]+)/index.aspx", RegexOptions.IgnoreCase | RegexOptions.Singleline);
			var target = new DefaultOutputRuleAction(pattern, text);

			var condTest = new DefaultConditionTestValue("%{HTTP_HOST}");
			var condPattern = new Pattern(@"(www)\.(.*)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
			var cond = MockCond(condPattern, condTest, null);

			var httpContext = CreateHttpContext(new Uri("http://www.managedfusion.com/success/index.aspx")).SetServerVariables(new Dictionary<string, string> { 
				{ "HTTP_HOST", "www.managedfusion.com" } 
			});
			string content = CreateHtmlContent(@"<a href=""/success/index.aspx"" target=""_blank"">Link</a>");
			var rule = MockRule(new List<ICondition> { cond }, target, null);

			RuleContext context = CreateOutputRuleContext(content.ToByteArray(), httpContext, rule);
			target.Execute(context);

			string result = context.SubstitutedContent.GetString();
			string expected = CreateHtmlContent(@"<a href=""http://managedfusion.com/www/www.managedfusion.com/success"" target=""_blank"">Link</a>");

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void VerifyExecuteWithTenConditionalValues()
		{
			var text = "http://%3/%5/$1";
			var pattern = new Pattern("/([a-z]+)/index.aspx", RegexOptions.IgnoreCase | RegexOptions.Singleline);
			var target = new DefaultOutputRuleAction(pattern, text);

			var condTest = new DefaultConditionTestValue("%{HTTP_HOST}");
			var condPattern = new Pattern(@"(www)\.(.*)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
			var cond = MockCond(condPattern, condTest, null);

			var condTest2 = new DefaultConditionTestValue("goto-%{SERVER_PORT}.aspx");
			var condPattern2 = new Pattern(@"goto-([0-9]{3,4}).aspx", RegexOptions.IgnoreCase | RegexOptions.Singleline);
			var cond2 = MockCond(condPattern2, condTest2, null);

			var httpContext = CreateHttpContext(new Uri("http://www.managedfusion.com/success/index.aspx")).SetServerVariables(new Dictionary<string, string> { 
				{ "HTTP_HOST", "www.managedfusion.com" },
				{ "SERVER_PORT", "1234" }
			});
			string content = CreateHtmlContent(@"<a href=""/success/index.aspx"" target=""_blank"">Link</a>");
			var rule = MockRule(new List<ICondition> { cond, cond2 }, target, null);

			RuleContext context = CreateOutputRuleContext(content.ToByteArray(), httpContext, rule);
			target.Execute(context);

			string result = context.SubstitutedContent.GetString();
			string expected = CreateHtmlContent(@"<a href=""http://managedfusion.com/1234/success"" target=""_blank"">Link</a>");

			Assert.AreEqual(expected, result);
		}
	}
}