using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using Moq;
using NUnit.Framework;
using ManagedFusion.Rewriter.Engines;
using System.Text.RegularExpressions;

namespace ManagedFusion.Rewriter.Tests
{
	public class BaseTest
	{
		/// <summary>
		/// Creates the rule set.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns></returns>
		public RuleSet CreateRuleSet(string text)
		{
			var ruleSet = new ApacheRuleSet("/", null);
			var reader = new StringReader("RewriteEngine On\n" + text);
			ruleSet.RefreshRules(reader);

			return ruleSet;
		}

		/// <summary>
		/// Creates the HTML content for testing.
		/// </summary>
		/// <param name="body">The body of the HTML content.</param>
		/// <returns>Returns a valid HTML content page.</returns>
		public string CreateHtmlContent(string body)
		{
			return @"<html><head><title>Test</title></head><body>" + body + "</body></html>";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="conditions"></param>
		/// <param name="action"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		public IRule MockRule(IList<ICondition> conditions, IRuleAction action, IRuleFlagProcessor flags)
		{
			// create properties
			if (conditions == null)
				conditions = new Mock<IList<ICondition>>().Object;
			
			if (action == null)
				action = new Mock<IRuleAction>().Object;

			if (flags == null)
			{
				var flagsMock = new Mock<IRuleFlagProcessor>();
				flagsMock.Expect(x => x.GetEnumerator()).Returns(() => {
					return new List<IRuleFlag>(0).GetEnumerator();
				});
				flags = flagsMock.Object;
			}

			var ruleMock = new Mock<IRule>();
			ruleMock.SetupGet(r => r.Conditions).Returns(conditions);
			ruleMock.SetupGet(r => r.Action).Returns(action);
			ruleMock.SetupGet(r => r.Flags).Returns(flags);
			var rule = ruleMock.Object;

			return rule;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pattern"></param>
		/// <param name="test"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		public ICondition MockCond(Pattern pattern, IConditionTestValue test, IConditionFlagProcessor flags)
		{
			// create properties
			if (pattern == null)
				pattern = new Pattern(".*", RegexOptions.Singleline);

			if (test == null)
				test = new Mock<IConditionTestValue>().Object;

			if (flags == null)
				flags = new Mock<IConditionFlagProcessor>().Object;

			var condMock = new Mock<ICondition>();
			condMock.SetupGet(c => c.Pattern).Returns(pattern);
			condMock.SetupGet(c => c.Test).Returns(test);
			condMock.SetupGet(c => c.Flags).Returns(flags);
			var cond = condMock.Object;

			return cond;
		}

		/// <summary>
		/// Creates the rule execution context.
		/// </summary>
		/// <param name="requestedUrl"></param>
		/// <returns></returns>
		public RuleContext CreateOutputRuleContext(Uri requestedUrl, byte[] responseContent)
		{
			var httpContext = HttpHelpers.MockHttpContext(requestedUrl);
			return CreateOutputRuleContext(responseContent, httpContext);
		}

		/// <summary>
		/// Create the rule exection context.
		/// </summary>
		/// <param name="responseContent"></param>
		/// <param name="httpContext"></param>
		/// <returns></returns>
		public RuleContext CreateOutputRuleContext(byte[] responseContent, HttpContextBase httpContext)
		{
			var rule = MockRule(null, null, null);

			return CreateOutputRuleContext(responseContent, httpContext, rule);
		}

		/// <summary>
		/// Create the rule exection context.
		/// </summary>
		/// <param name="responseContent"></param>
		/// <param name="httpContext"></param>
		/// <param name="rule"></param>
		/// <returns></returns>
		public RuleContext CreateOutputRuleContext(byte[] responseContent, HttpContextBase httpContext, IRule rule)
		{
			var ruleSet = CreateRuleSet(String.Empty);

			// setup object
			var ruleSetContext = new RuleSetContext(ruleSet, responseContent, httpContext);
			var ruleContext = new RuleContext(0, ruleSetContext, responseContent, rule);

			// return object
			return ruleContext;
		}

		/// <summary>
		/// Creates the rule execution context.
		/// </summary>
		/// <param name="requestedUrl"></param>
		/// <returns></returns>
		public RuleContext CreateRuleContext(Uri requestedUrl)
		{
			var httpContext = HttpHelpers.MockHttpContext(requestedUrl);
			return CreateRuleContext(requestedUrl, httpContext);
		}

		/// <summary>
		/// Creates the rule execution context.
		/// </summary>
		/// <param name="requestedUrl"></param>
		/// <param name="httpContext"></param>
		/// <returns></returns>
		public RuleContext CreateRuleContext(Uri requestedUrl, HttpContextBase httpContext)
		{
			var rule = MockRule(null, null, null);

			// return object
			return CreateRuleContext(requestedUrl, httpContext, rule);
		}

		/// <summary>
		/// Creates the rule exection context.
		/// </summary>
		/// <param name="requestedUrl"></param>
		/// <param name="httpContext"></param>
		/// <param name="rule"></param>
		/// <returns></returns>
		public RuleContext CreateRuleContext(Uri requestedUrl, HttpContextBase httpContext, IRule rule)
		{
			var ruleSet = CreateRuleSet(String.Empty);

			// setup object
			var ruleSetContext = new RuleSetContext(ruleSet, requestedUrl, httpContext);
			var ruleContext = new RuleContext(0, ruleSetContext, requestedUrl, rule);

			// return object
			return ruleContext;
		}

		/// <summary>
		/// Creates the condition execution context.
		/// </summary>
		/// <returns></returns>
		public ConditionContext CreateOutputConditionContext(Uri requestedUrl, byte[] responseContent)
		{
			// create properties
			var condition = new Mock<ICondition>().Object;

			// setup object
			var ruleContext = CreateOutputRuleContext(requestedUrl, responseContent);
			var conditionContext = new ConditionContext(0, ruleContext, condition);

			// return object
			return conditionContext;
		}

		/// <summary>
		/// Creates the condition execution context with condition values.
		/// </summary>
		/// <param name="requestedUrl">The requested URL.</param>
		/// <param name="conditionValues">The condition values.</param>
		/// <returns></returns>
		public ConditionContext CreateOutputConditionContextWithConditionValues(Uri requestedUrl, byte[] responseContent, string[] conditionValues)
		{
			// get condition context
			var conditionContext = CreateOutputConditionContext(requestedUrl, responseContent);

			// mock conditions count
			var conditionsMock = Mock.Get(conditionContext.Conditions);
			conditionsMock.SetupGet(c => c.Count).Returns(conditionValues.Length);

			// mock object
			var mock = new Mock<ConditionContext>(MockBehavior.Strict, 0, (RuleContext)conditionContext, conditionContext.CurrentCondition);
			int maxIndex = conditionValues.Length - 1;
			mock.SetupGet(c => c.Conditions).Returns(conditionsMock.Object);
			mock.SetupGet(c => c.LogLevel).Returns(1);
			mock.Expect(c => c.GetConditionValue(It.IsInRange(0, maxIndex, Range.Inclusive))).Returns((int i) => conditionValues[i]);

			// create object
			return mock.Object;
		}

		/// <summary>
		/// Creates the condition execution context.
		/// </summary>
		/// <returns></returns>
		public ConditionContext CreateConditionContext(Uri requestedUrl)
		{
			// create properties
			var condition = new Mock<ICondition>().Object;

			// setup object
			var ruleContext = CreateRuleContext(requestedUrl);
			var conditionContext = new ConditionContext(0, ruleContext, condition);

			// return object
			return conditionContext;
		}

		/// <summary>
		/// Creates the condition execution context with condition values.
		/// </summary>
		/// <param name="requestedUrl">The requested URL.</param>
		/// <param name="conditionValues">The condition values.</param>
		/// <returns></returns>
		public ConditionContext CreateConditionContextWithConditionValues(Uri requestedUrl, string[] conditionValues)
		{
			// get condition context
			var conditionContext = CreateConditionContext(requestedUrl);

			// mock conditions count
			var conditionsMock = Mock.Get(conditionContext.Conditions);
			conditionsMock.SetupGet(c => c.Count).Returns(conditionValues.Length);
			
			// mock object
			var mock = new Mock<ConditionContext>(MockBehavior.Strict, 0, (RuleContext)conditionContext, conditionContext.CurrentCondition);
			int maxIndex = conditionValues.Length - 1;
			mock.SetupGet(c => c.Conditions).Returns(conditionsMock.Object);
			mock.SetupGet(c => c.LogLevel).Returns(1);
			mock.Expect(c => c.GetConditionValue(It.IsInRange(0, maxIndex, Range.Inclusive))).Returns((int i) => conditionValues[i]);

			// create object
			return mock.Object;
		}
	}
}
