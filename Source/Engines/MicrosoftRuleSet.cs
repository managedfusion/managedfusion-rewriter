/** 
 * Copyright (C) 2007-2010 Nicholas Berardi, Managed Fusion, LLC (nick@managedfusion.com)
 * 
 * <author>Nicholas Berardi</author>
 * <author_email>nick@managedfusion.com</author_email>
 * <company>Managed Fusion, LLC</company>
 * <product>Url Rewriter and Reverse Proxy</product>
 * <license>Microsoft Public License (Ms-PL)</license>
 * <agreement>
 * This software, as defined above in <product />, is copyrighted by the <author /> and the <company />, all defined above.
 * 
 * For all binary distributions the <product /> is licensed for use under <license />.
 * For all source distributions please contact the <author /> at <author_email /> for a commercial license.
 * 
 * This copyright notice may not be removed and if this <product /> or any parts of it are used any other
 * packaged software, attribution needs to be given to the author, <author />.  This can be in the form of a textual
 * message at program startup or in documentation (online or textual) provided with the packaged software.
 * </agreement>
 * <product_url>http://www.managedfusion.com/products/url-rewriter/</product_url>
 * <license_url>http://www.managedfusion.com/products/url-rewriter/license.aspx</license_url>
 */

using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

using ManagedFusion.Rewriter.Conditions;
using ManagedFusion.Rewriter.Rules;

using RF = ManagedFusion.Rewriter.Rules.Flags;
using CF = ManagedFusion.Rewriter.Conditions.Flags;

namespace ManagedFusion.Rewriter.Engines
{
	/// <summary>
	/// 
	/// </summary>
	internal class MicrosoftRuleSet : RuleSet
	{
		private FileInfo _ruleSetConfig;

		/// <summary>
		/// Initializes a new instance of the <see cref="MicrosoftRuleSet"/> class.
		/// </summary>
		/// <param name="ruleSetConfig">The rule set config.</param>
		public MicrosoftRuleSet(FileInfo ruleSetConfig)
		{
			PhysicalBase = "/";
			_ruleSetConfig = ruleSetConfig;
		}

		/// <summary>
		/// Refreshes the rules.
		/// </summary>
		public void RefreshRules()
		{
			using (StreamReader reader = _ruleSetConfig.OpenText())
			{
				RefreshRules(reader, "/configuration/system.webServer/rewrite/rules/rule");
			}
		}

		/// <summary>
		/// Refreshes the rules.
		/// </summary>
		/// <param name="reader">The reader.</param>
		public void RefreshRules(StreamReader reader, string xpath)
		{
			XmlDocument configuration = new XmlDocument();

			// load the configuration
			configuration.Load(reader);

			// get the rules
			XmlNodeList rules = configuration.SelectNodes(xpath);

			AddRules(rules);
		}

		/// <summary>
		/// Adds the rules.
		/// </summary>
		/// <param name="ruleElements">The rule elements.</param>
		public void AddRules(XmlNodeList ruleElements)
		{
			foreach (XmlNode ruleElement in ruleElements)
			{
				IRule rule = GetRule(ruleElement);

				if (rule != null)
					AddRule(rule);
			}
		}

		/// <summary>
		/// Gets the rule.
		/// </summary>
		/// <param name="ruleElement">The rule element.</param>
		/// <returns></returns>
		private IRule GetRule(XmlNode ruleElement)
		{
			if (ruleElement == null)
				throw new ArgumentNullException("ruleElement");

			if (ruleElement.Name != "rule")
				throw new RuleSetException("The node is not a \"rule\".");

			bool enabled = true; // from schema definition

			if (ruleElement.Attributes["enabled"] != null)
				enabled = XmlConvert.ToBoolean(ruleElement.Attributes["enabled"].Value);

			// if it is not enabled there is no reason to continue processing
			if (!enabled)
				return null;

			string name = String.Empty;
			bool stopProcessing = false; // from schema definition
			string patternSyntax = "ECMAScript"; // from schema definiton

			if (ruleElement.Attributes["name"] != null)
				name = ruleElement.Attributes["name"].Value;

			if (ruleElement.Attributes["stopProcessing"] != null)
				stopProcessing = XmlConvert.ToBoolean(ruleElement.Attributes["stopProcessing"].Value);

			if (ruleElement.Attributes["patternSyntax"] != null)
				patternSyntax = ruleElement.Attributes["patternSyntax"].Value;

			XmlNode matchElement = ruleElement.SelectSingleNode("match");
			XmlNode conditionsElement = ruleElement.SelectSingleNode("conditions");
			XmlNode serverVariablesElement = ruleElement.SelectSingleNode("serverVariables");
			XmlNode actionElement = ruleElement.SelectSingleNode("action");

			IRuleFlagProcessor ruleFlags = new RuleFlagProcessor();
			IRule rule = new DefaultRule();
			rule.Name = name;

			// <match />
			Pattern match = GetMatch(matchElement, ref ruleFlags);

			// <condition />
			IEnumerable<ICondition> conditions = GetConditions(conditionsElement);

			// <serverVariables />
			foreach (var flag in GetServerVariables(serverVariablesElement))
				ruleFlags.Add(flag);

			// <action />
			IRuleAction action = GetAction(actionElement, match, ref ruleFlags);

			// <rule />
			rule.Init(conditions, action, ruleFlags);

			return rule;
		}

		private enum ActionType
		{
			None = 0,
			Rewrite,
			Redirect,
			CustomResponse,
			AbortRequest
		}

		private enum RedirectType
		{
			Permanent = 301,
			Found = 302,
			SeeOther = 303,
			Temporary = 307
		}

		/// <summary>
		/// Gets the action.
		/// </summary>
		/// <param name="actionElement">The action element.</param>
		/// <param name="ruleFlags">The rule flags.</param>
		/// <returns></returns>
		private IRuleAction GetAction(XmlNode actionElement, Pattern pattern, ref IRuleFlagProcessor ruleFlags)
		{
			ActionType type = ActionType.None;
			string url = null;
			bool appendQueryString = true; // from schema definition
			RedirectType redirectType = RedirectType.Permanent;
			uint statusCode = 0U;
			uint subStatusCode = 0U; // from schema definition
			string statusReason = null;
			string statusDescription = null;

			if (actionElement.Attributes["type"] != null)
			{
				try { type = (ActionType)Enum.Parse(typeof(ActionType), actionElement.Attributes["type"].Value, true); }
				catch (Exception exc) { Manager.Log("Action: " + exc.Message, "Error"); }
			}

			if (actionElement.Attributes["url"] != null)
				url = actionElement.Attributes["url"].Value;

			if (actionElement.Attributes["appendQueryString"] != null)
				appendQueryString = XmlConvert.ToBoolean(actionElement.Attributes["appendQueryString"].Value);

			if (actionElement.Attributes["redirectType"] != null)
			{
				try { redirectType = (RedirectType)Enum.Parse(typeof(RedirectType), actionElement.Attributes["redirectType"].Value, true); }
				catch (Exception exc) { Manager.Log("Action: " + exc.Message, "Error"); }
			}

			if (actionElement.Attributes["statusCode"] != null)
				statusCode = XmlConvert.ToUInt32(actionElement.Attributes["statusCode"].Value);

			if (actionElement.Attributes["subStatusCode"] != null)
				subStatusCode = XmlConvert.ToUInt32(actionElement.Attributes["subStatusCode"].Value);

			if (actionElement.Attributes["statusReason"] != null)
				statusReason = actionElement.Attributes["statusReason"].Value;

			if (actionElement.Attributes["statusDescription"] != null)
				statusDescription = actionElement.Attributes["statusDescription"].Value;

			if (String.IsNullOrEmpty(url))
				throw new RuleSetException("Action URL must be a non-empty value.");

			// validationType="requireTrimmedString"
			url = url.Trim();

			if (type == ActionType.Redirect)
			{
				ruleFlags.Add(new RF.RedirectFlag((int)redirectType));
			}
			else if (statusCode > 0U)
			{
				// validationType="integerRange" validationParameter="300,307,exclude"
				if (statusCode >= 300U && statusCode <= 307U)
					throw new RuleSetException("Action Status Code should not be an int between 300 - 307, use the redirectType for this range.");

				if (statusCode < 1U || statusCode > 999U)
					throw new RuleSetException("Action Status Code should be between 1 - 999.");

				if (subStatusCode < 0U || subStatusCode > 999U)
					throw new RuleSetException("Action Sub Status Code should be between 0 - 999.");

				ruleFlags.Add(new RF.ResponseStatusFlag(statusCode, subStatusCode, statusReason, statusDescription));
			}

			IRuleAction substitution = new DefaultRuleAction();
			substitution.Init(pattern, url);

			return substitution;
		}

		/// <summary>
		/// Gets the server variables.
		/// </summary>
		/// <param name="serverVariablesElement">The server variables element.</param>
		/// <param name="ruleFlags">The rule flags.</param>
		/// <returns></returns>
		private IEnumerable<RF.ServerVariableFlag> GetServerVariables(XmlNode serverVariablesElement)
		{
			// process each server variable
			foreach (XmlNode serverVariableElement in serverVariablesElement.SelectNodes("set"))
				yield return GetServerVariable(serverVariableElement);
		}

		/// <summary>
		/// Gets the server variable.
		/// </summary>
		/// <param name="serverVariableElement">The server variable element.</param>
		/// <returns></returns>
		private RF.ServerVariableFlag GetServerVariable(XmlNode serverVariableElement)
		{
			string name = null;
			string value = String.Empty;
			bool replace = true; // from schema definition

			if (serverVariableElement.Attributes["name"] != null)
				name = serverVariableElement.Attributes["name"].Value;

			if (serverVariableElement.Attributes["value"] != null)
				value = serverVariableElement.Attributes["value"].Value;

			if (serverVariableElement.Attributes["replace"] != null)
				replace = XmlConvert.ToBoolean(serverVariableElement.Attributes["replace"].Value);

			// required="true"
			if (String.IsNullOrEmpty(name))
				throw new RuleSetException("Server Variable Name must be a non-empty value.");

			// validationType="requireTrimmedString"
			name = name.Trim();

			return new RF.ServerVariableFlag(name, value, replace);
		}

		/// <summary>
		/// Gets the match.
		/// </summary>
		/// <param name="matchElement">The match element.</param>
		/// <returns></returns>
		private Pattern GetMatch(XmlNode matchElement, ref IRuleFlagProcessor ruleFlags)
		{
			string url = null;
			bool ignoreCase = true; // from schema definition
			bool negate = false; // from schema definition

			if (matchElement.Attributes["url"] != null)
				url = matchElement.Attributes["url"].Value;

			if (matchElement.Attributes["ignoreCase"] != null)
				ignoreCase = XmlConvert.ToBoolean(matchElement.Attributes["ignoreCase"].Value);

			if (matchElement.Attributes["negate"] != null)
				negate = XmlConvert.ToBoolean(matchElement.Attributes["negate"].Value);

			// validationType="nonEmptyString"
			if (String.IsNullOrEmpty(url))
				throw new RuleSetException("Match URL must be a non-empty value.");

			var patternOptions = Manager.RuleOptions;

			if (ignoreCase)
			{
				ruleFlags.Add(new RF.NoCaseFlag());
				patternOptions |= RegexOptions.IgnoreCase;
			}

			return new Pattern(url, negate, patternOptions);
		}

		private enum LogicalGrouping
		{
			MatchAll = 0,
			MatchAny
		}

		/// <summary>
		/// Gets the conditions.
		/// </summary>
		/// <param name="conditionsElement">The conditions element.</param>
		/// <returns></returns>
		private IEnumerable<ICondition> GetConditions(XmlNode conditionsElement)
		{
			LogicalGrouping logicalGrouping = LogicalGrouping.MatchAll; // from schema definition
			bool trackAllCaptures = false; // from schema definition

			if (conditionsElement.Attributes["logicalGrouping"] != null)
			{
				try { logicalGrouping = (LogicalGrouping)Enum.Parse(typeof(LogicalGrouping), conditionsElement.Attributes["logicalGrouping"].Value, true); }
				catch (Exception exc) { Manager.Log("Condition: " + exc.Message, "Error"); }
			}

			if (conditionsElement.Attributes["trackAllCaptures"] != null)
				trackAllCaptures = XmlConvert.ToBoolean(conditionsElement.Attributes["trackAllCaptures"].Value);

			// process each condition
			foreach (XmlNode conditionElement in conditionsElement.SelectNodes("add"))
				yield return GetCondition(conditionElement, logicalGrouping);
		}

		private enum MatchType
		{
			Pattern = 0,
			IsFile,
			IsDirectory
		}

		/// <summary>
		/// Gets the condition.
		/// </summary>
		/// <param name="conditionElement">The condition element.</param>
		/// <param name="matchAll">if set to <see langword="true"/> [match all].</param>
		/// <returns></returns>
		private ICondition GetCondition(XmlNode conditionElement, LogicalGrouping logicalGrouping)
		{
			string input = "-";
			MatchType matchType = MatchType.Pattern; // from schema definition
			string pattern = "(.*)";
			bool ignoreCase = true; // from schema definition
			bool negate = false; // from schema definition

			if (conditionElement.Attributes["input"] != null)
				input = conditionElement.Attributes["input"].Value;

			if (conditionElement.Attributes["matchType"] != null)
			{
				try { matchType = (MatchType)Enum.Parse(typeof(MatchType), conditionElement.Attributes["matchType"].Value, true); }
				catch (Exception exc) { Manager.Log("Condition: " + exc.Message, "Error"); }
			}

			if (conditionElement.Attributes["pattern"] != null)
				pattern = conditionElement.Attributes["pattern"].Value;

			if (conditionElement.Attributes["ignoreCase"] != null)
				ignoreCase = XmlConvert.ToBoolean(conditionElement.Attributes["ignoreCase"].Value);

			if (conditionElement.Attributes["negate"] != null)
				negate = XmlConvert.ToBoolean(conditionElement.Attributes["negate"].Value);

			RegexOptions conditionOptions = Manager.RuleOptions;
			IConditionFlagProcessor conditionFlags = new ConditionFlagProcessor();

			if (ignoreCase)
			{
				conditionFlags.Add(new CF.NoCaseFlag());
				conditionOptions |= RegexOptions.IgnoreCase;
			}

			if (logicalGrouping == LogicalGrouping.MatchAny)
				conditionFlags.Add(new CF.OrNextFlag());

			ICondition condition = null;

			// create the condition
			switch (matchType)
			{
				case MatchType.IsFile: condition = new IsFileCondition(); break;
				case MatchType.IsDirectory: condition = new IsDirectoryCondition(); break;
				case MatchType.Pattern: condition = new DefaultCondition(); break;
			}

			Pattern compiledPattern = new Pattern(pattern, negate, conditionOptions);
			var conditionTest = new DefaultConditionTestValue(input);

			// initialize condition
			condition.Init(compiledPattern, conditionTest, conditionFlags);

			return condition;
		}
	}
}
