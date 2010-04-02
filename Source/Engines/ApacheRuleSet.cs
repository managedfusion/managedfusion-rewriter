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
using System.IO;
using System.Text.RegularExpressions;
using System.Security;
using System.Web.Compilation;
using System.Web.Hosting;

using ManagedFusion.Rewriter.Conditions;
using ManagedFusion.Rewriter.Rules;

using RF = ManagedFusion.Rewriter.Rules.Flags;
using CF = ManagedFusion.Rewriter.Conditions.Flags;

namespace ManagedFusion.Rewriter.Engines
{
	public class ApacheRuleSet : RuleSet
	{
		private static readonly RegexOptions FileOptions = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant;

		/// <summary>Rewrite Log Level</summary>
		/// <remarks>
		/// 0 : Nothing is logged.
		/// 1 : Provides simple input and output.
		/// 2 : Provides 1 and conditions and rewrites input and ouput.
		/// 3 : Provides 2 and processing rules from rules file.
		/// 4 : Provides 3 and ignored lines.
		/// 5 : Provides 4 and shows configuration errors
		/// ...
		/// 9 : Provides 8 and is very verbose.
		/// </remarks>
		private static readonly Regex RewriteLogLevelLine = new Regex(@"^RewriteLogLevel[\s]+(?<level>[0-9])", FileOptions);
		private static readonly Regex RewriteModuleLine = new Regex(@"^RewriteModule[\s]+(?<name>[\S]+)[\s]+(?<type>(.*))", FileOptions);
		private static readonly Regex RewriteLogLine = new Regex(@"^RewriteLog[\s]+""?(?<location>[^""]+)""?", FileOptions);
		private static readonly Regex RewriteEngineLine = new Regex(@"^RewriteEngine[\s]+(?i:(?<state>on|off))", FileOptions);
		private static readonly Regex RewriteOptionsLine = new Regex(@"^RewriteOptions[\s]+((?<var>\w*=\w*)[\s]*)*", FileOptions);
		private static readonly Regex RewriteBaseLine = new Regex(@"^RewriteBase[\s]+(?<base>/(.*))", FileOptions);
		private static readonly Regex RewriteCondLine = new Regex(@"^RewriteCond(\((?<module1>\w*)\,(?<module2>\w*)\))?[\s]+(?<test>[\S]+)[\s]+(?<pattern>[\S]+)[\s]*(\[(?<flags>[\s\w][^\]]*)\])?", FileOptions);
		private static readonly Regex RewriteRuleLine = new Regex(@"^RewriteRule(\((?<module1>\w*)\,(?<module2>\w*)\))?[\s]+(?<pattern>[\S]+)[\s]+(""(?<substitution>[^""]+)""|(?<substitution>[\S]+))[\s]*(\[(?<flags>[\s\w][^\]]*)\])?", FileOptions);

		private static readonly Regex OutRewriteCondLine = new Regex(@"^OutRewriteCond(\((?<module1>\w*)\,(?<module2>\w*)\))?[\s]+(?<test>[\S]+)[\s]+(?<pattern>[\S]+)[\s]*(\[(?<flags>[\s\w][^\]]*)\])?", FileOptions);
		private static readonly Regex OutRewriteRuleLine = new Regex(@"^OutRewriteRule(\((?<module1>\w*)\,(?<module2>\w*)\))?[\s]+(?<pattern>[\S]+)[\s]+(""(?<substitution>[^""]+)""|(?<substitution>[\S]+))[\s]*(\[(?<flags>[\s\w][^\]]*)\])?", FileOptions);

		private static readonly Regex HtmlTagExpression = new Regex(@"(?'tag_start'</?)(?'tag'\w+)((\s+(?'attr'(?'attr_name'\w+)(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+)))?)+\s*|\s*)(?'tag_end'/?>)", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
		private static readonly Dictionary<string, List<string>> TagAndAttributes = new Dictionary<string, List<string>> {
			{ "a", new List<string> { "href" } }, 
			{ "area", new List<string> { "href" } }, 
			{ "base", new List<string> { "href" } }, 
			{ "form", new List<string> { "action" } }, 
			{ "frame", new List<string> { "src", "longdesc" } }, 
			{ "head", new List<string> { "profile" } }, 
			{ "iframe", new List<string> { "src", "longdesc" } }, 
			{ "img", new List<string> { "src", "longdesc", "usemap" } }, 
			{ "input", new List<string> { "src", "usemap" } }, 
			{ "link", new List<string> { "href" } }, 
			{ "script", new List<string> { "src" } }
		};

		private readonly FileInfo _ruleSetFile;
		private object _refreshLock;

		/// <summary>
		/// Initializes a new instance of the <see cref="ApacheRuleSet"/> class.
		/// </summary>
		/// <param name="physicalBase">The physical base.</param>
		/// <param name="ruleSetFile">The rule set file.</param>
		public ApacheRuleSet(string physicalBase, FileInfo ruleSetFile)
		{
			PhysicalBase = physicalBase;

			_ruleSetFile = ruleSetFile;
			_refreshLock = new Object();
		}

		#region Initialize Rules

		/// <summary>
		/// Splits the flag.
		/// </summary>
		/// <param name="flag">The flag.</param>
		/// <returns></returns>
		private string[] SplitFlag(string flag)
		{
			if (flag == null)
				return null;

			int equalIndex = flag.IndexOf('=');

			if (equalIndex == 0)
				return null;

			string[] temporarySplit = flag.Split(new char[] { '=' }, 2, StringSplitOptions.None);
			string key = temporarySplit[0].Trim();
			string value = temporarySplit.Length > 1 ? temporarySplit[1].Trim() : String.Empty;

			//if (String.IsNullOrEmpty(value))
			//    value = key;

			return new string[] { key, value };
		}

		/// <summary>
		/// Adds the condition flag.
		/// </summary>
		/// <param name="flag">The flag.</param>
		/// <returns></returns>
		private IConditionFlag AddConditionFlag(string flag)
		{
			string[] temp = SplitFlag(flag);
			string key = temp[0];
			//string value = temp[1];

			switch (key)
			{
				case "nocase":
				case "NC":
					return new CF.NoCaseFlag();
				case "ornext":
				case "OR":
					return new CF.OrNextFlag();
			}

			Manager.LogIf(LogLevel >= 5, "Error: No flag match was found for " + key, "Rule Processing");
			return null;
		}

		/// <summary>
		/// Splits the condition flags.
		/// </summary>
		/// <param name="flags">The flags.</param>
		/// <returns></returns>
		private IConditionFlagProcessor SplitConditionFlags(string flags)
		{
			IConditionFlagProcessor dictionary = new ConditionFlagProcessor();

			foreach (string flag in flags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
			{
				IConditionFlag conditionFlag = AddConditionFlag(flag);
				if (conditionFlag != null)
					dictionary.Add(conditionFlag);
			}

			return dictionary;
		}

		/// <summary>
		/// Adds the rule flag.
		/// </summary>
		/// <param name="flag">The flag.</param>
		/// <returns></returns>
		private IRuleFlag AddRuleFlag(string flag)
		{
			string[] temp = SplitFlag(flag);
			string key = temp[0];
			string value = temp[1];

			switch (key)
			{
				case "chain":
				case "C":
					return new RF.ChainFlag();
				case "cookie":
				case "CO":
					{
						if (value == null)
							return null;

						string[] info = value.Split(new char[] { ':' }, 5, StringSplitOptions.None);
						string name = info[0],
							   val = info[1],
							   domain = info.Length > 2 ? info[2].Trim() : String.Empty,
							   path = info.Length > 4 ? info[4].Trim() : String.Empty;
						TimeSpan? expires = null;
						int expiresMinutes;

						if (info.Length > 3 && Int32.TryParse(info[3], out expiresMinutes))
							expires = new TimeSpan(0, expiresMinutes, 0);

						return new RF.ResponseCookieFlag(name, val, domain, expires, path);
					}
				//case "env":
				//case "E": key = "env"; break;
				case "forbidden":
				case "F":
					return new RF.ResponseStatusFlag("forbidden");
				case "notfound":
				case "NF":
					return new RF.ResponseStatusFlag("notfound");
				case "gone":
				case "G":
					return new RF.ResponseStatusFlag("gone");
				case "last":
				case "L":
					return new RF.LastFlag();
				case "next":
				case "N":
					return new RF.NextRuleFlag();
				case "nocase":
				case "NC":
					return new RF.NoCaseFlag();
				case "noescape":
				case "NE":
					return new RF.NoEscapeFlag();
				case "nosubreq":
				case "NS":
					return new RF.NotForInternalSubRequestsFlag();	
				case "proxy":
				case "P":
					return new RF.ProxyFlag();
				//case "passthrough":
				//case "PT": key = "passthrough"; break;
				case "qsappend":
				case "QSA":
					return new RF.QueryStringAppendFlag();
				case "redirect":
				case "R":
					return new RF.RedirectFlag(value);
				case "skip":
				case "S":
					{
						int skipIndex;
						if (Int32.TryParse(value, out skipIndex))
							return new RF.SkipRuleFlag(skipIndex);

						Manager.LogIf(LogLevel >= 5, "Error: " + value + " is not a valid skip index.", "Rule Processing");
						return null;
					}
				case "type":
				case "T":
					return new RF.ResponseMimeTypeFlag(value);
			}

			Manager.LogIf(LogLevel >= 5, "Error: No flag match was found for " + key, "Rule Processing");
			return null;
		}

		/// <summary>
		/// Splits the rule flags.
		/// </summary>
		/// <param name="flags">The flags.</param>
		/// <returns></returns>
		private IRuleFlagProcessor SplitRuleFlags(string flags)
		{
			RuleFlagProcessor dictionary = new RuleFlagProcessor();
			List<string> temporaryHolding = new List<string>();

			dictionary.BeginAdd();
			foreach (string flag in flags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
			{
				string temp = flag;

				if (flag.Contains("\"") || temporaryHolding.Count > 0)
				{
					temporaryHolding.Add(flag.Replace("\"", String.Empty));

					if (flag.Contains("\"") && temporaryHolding.Count > 1)
					{
						temp = String.Join(",", temporaryHolding.ToArray());
						temporaryHolding.Clear();
					}
					else
						continue;
				}

				IRuleFlag ruleFlag = AddRuleFlag(temp);
				if (ruleFlag != null)
					dictionary.Add(ruleFlag);
			}
			dictionary.EndAdd();

			return dictionary;
		}

		/// <summary>
		/// Gets the condition.
		/// </summary>
		/// <param name="pattern">The pattern.</param>
		/// <returns></returns>
		private ICondition GetCondition(string pattern)
		{
			if (String.IsNullOrEmpty(pattern))
				return null;

			// check for an invert and remove it
			if (pattern.StartsWith("!"))
				pattern = pattern.Substring(1);

			// find the condition to create
			switch (pattern[0])
			{
				case '<':
					return new LexicographicallyPrecedesCondition();
				case '>':
					return new LexicographicallyFollowsCondition();
				case '=':
					return new LexicographicallyEqualCondition();
				case '-':
					switch (pattern)
					{
						case "-d":
							return new IsDirectoryCondition();
						case "-f":
							return new IsFileCondition();
						case "-s":
							return new IsFileWithSizeCondition();

						//TODO: none of these are implimented
						case "-l":
						case "-F":
						case "-U":
							goto default;

						default:
							return new DefaultCondition();
					}

				default:
					return new DefaultCondition();
			}
		}

		/// <summary>
		/// Gets the condition.
		/// </summary>
		/// <param name="test">The test.</param>
		/// <returns></returns>
		private IConditionTestValue GetConditionTestValue(ref string test)
		{
			return new DefaultConditionTestValue(test);
		}

		#endregion

		/// <summary>
		/// Refreshes the rules.
		/// </summary>
		public void RefreshRules()
		{
			using (StreamReader reader = _ruleSetFile.OpenText())
			{
				RefreshRules(reader);
			}
		}

		/// <summary>
		/// Refreshes the rules.
		/// </summary>
		/// <param name="reader">The reader.</param>
		public void RefreshRules(TextReader reader)
		{
			// put a lock on the refresh process so that only one refresh can happen at a time
			lock (_refreshLock)
			{
				Manager.LogEnabled = false;
				Manager.LogPath = null;

				string tempBase = PhysicalBase;
				string tempLogPath = null;
				int tempLogLevel = 0;
				int tempMaxInternalTransfers = 10;
				bool tempEngineEnabled = false;

				string line;
				IList<ICondition> conditions = new List<ICondition>(0);
				IList<IRule> rules = new List<IRule>();
				IList<IRule> outputRules = new List<IRule>();
				IList<string> unknownLines = new List<string>();
				ModuleFactory modules = new ModuleFactory();

				while (reader.Peek() >= 0)
				{
					line = reader.ReadLine().Trim();

					if (String.IsNullOrEmpty(line))
					{
						// just plain old ignore empty lines no logging or anything
						continue;
					}
					else if (line[0] == '#')
					{
						Manager.LogIf(tempLogLevel >= 4, "Comment: " + line, "Rule Processing");
					}
					else if (RewriteEngineLine.IsMatch(line))
					{
						#region RewriteEngine

						Match match = RewriteEngineLine.Match(line);
						string engineState = match.Groups["state"].Value;

						// by default the engine is turned off
						if (String.IsNullOrEmpty(engineState) || String.Equals(engineState, "off", StringComparison.OrdinalIgnoreCase))
						{
							rules.Clear();
							tempEngineEnabled = false;

							// don't bother processing any other rules if the engine is disabled
							break;
						}
						else
						{
							tempEngineEnabled = true;
						}

						Manager.LogIf(tempLogLevel >= 3, "RewriteEngine: " + (tempEngineEnabled ? "Enabled" : "Disabled"), "Rule Processing");

						#endregion
					}
					else if (RewriteOptionsLine.IsMatch(line))
					{
						#region RewriteOptions

						Match match = RewriteOptionsLine.Match(line);
						Group variables = match.Groups["var"];

						if (variables.Success)
						{
							foreach (Capture var in variables.Captures)
							{
								string[] parts = var.Value.Split(new char[] { '=' }, 2);
								bool variableUnderstood = false;

								if (parts.Length == 2)
								{
									switch (parts[0])
									{
										case "MaxRedirects":
											int maxInternalTransfers;
											if (Int32.TryParse(parts[1], out maxInternalTransfers))
											{
												tempMaxInternalTransfers = maxInternalTransfers;
												variableUnderstood = true;
											}
											break;
									}
								}

								if (!variableUnderstood)
									Manager.LogIf(tempLogLevel >= 4, "Not Understood: " + var.Value, "Unknown");
							}
						}

						#endregion
					}
					else if (RewriteBaseLine.IsMatch(line))
					{
						#region RewriteBase

						Match match = RewriteBaseLine.Match(line);
						tempBase = match.Groups["base"].Value;

						Manager.LogIf(tempLogLevel >= 3, "RewriteBase: " + VirtualBase, "Rule Processing");

						#endregion
					}
					else if (RewriteModuleLine.IsMatch(line))
					{
						#region RewriteModule

						Match match = RewriteModuleLine.Match(line);
						string moduleName = match.Groups["name"].Value;
						string moduleType = match.Groups["type"].Value;
						Type module = Type.GetType(moduleType, false, true);

						if (module == null)
							module = BuildManager.GetType(moduleType, false, true);

						if (module == null)
						{
							Manager.LogIf(tempLogLevel >= 3, "RewriteModule: Error finding " + moduleType, "Rule Processing");
						}
						else
						{
							// add the module to the list
							modules.AddModule(moduleName, module);

							Manager.LogIf(tempLogLevel >= 3, "RewriteModule: " + moduleType, "Rule Processing");
						}

						#endregion
					}
					else if (RewriteLogLine.IsMatch(line))
					{
						#region RewriteLog

						Match match = RewriteLogLine.Match(line);
						tempLogPath = match.Groups["location"].Value;
						tempLogPath = NormalizeLogLocation(tempLogPath);

						Manager.LogIf(tempLogLevel >= 3, "RewriteLog: " + tempLogPath, "Rule Processing");

						#endregion
					}
					else if (RewriteLogLevelLine.IsMatch(line))
					{
						#region RewriteLogLevel

						Match match = RewriteLogLevelLine.Match(line);
						int logLevel = 1;

						if (!Int32.TryParse(match.Groups["level"].Value, out logLevel))
						{
							tempLogLevel = 0;
							Manager.LogIf(tempLogLevel >= 3, "RewriteLogLevel: " + match.Groups["level"].Value + " not understood.", "Rule Processing");
						}
						else
						{
							tempLogLevel = logLevel;
						}

						Manager.LogIf(tempLogLevel >= 3, "RewriteLogLevel: " + logLevel, "Rule Processing");

						#endregion
					}
					else if (RewriteCondLine.IsMatch(line))
					{
						#region RewriteCond

						Match match = RewriteCondLine.Match(line);

						string module1 = match.Groups["module1"].Value;
						string module2 = match.Groups["module2"].Value;

						Type moduleType1 = null;
						Type moduleType2 = null;

						// set the types of the first module
						if (modules.ContainsName(module1))
							moduleType1 = modules.GetModule(module1);

						// make sure the module is of the right type
						if (moduleType1 != null && moduleType1.GetInterface("ICondition", false) == null)
							moduleType1 = null;

						// set the types of the second module
						if (modules.ContainsName(module2))
							moduleType2 = modules.GetModule(module2);

						// make sure the module is of the right type
						if (moduleType2 != null && moduleType2.GetInterface("IConditionTestValue", false) == null)
							moduleType2 = null;

						try
						{
							RegexOptions patternOptions = Manager.RuleOptions;
							IConditionFlagProcessor flags;

							if (match.Groups["flags"] != null)
								flags = SplitConditionFlags(match.Groups["flags"].Value);
							else
								flags = new ConditionFlagProcessor();

							// check to see if the pattern should ignore the case when testing
							if (ConditionFlagsProcessor.HasNoCase(flags))
								patternOptions |= RegexOptions.IgnoreCase;

							string test = match.Groups["test"].Value;
							string pattern = match.Groups["pattern"].Value;
							IConditionTestValue testValue = null;
							ICondition condition = null;

							// create the second module
							if (moduleType2 == null)
								testValue = GetConditionTestValue(ref test);
							else
								testValue = Activator.CreateInstance(moduleType2) as IConditionTestValue;

							// create the first module
							if (moduleType1 == null)
								condition = GetCondition(pattern);
							else
								condition = Activator.CreateInstance(moduleType1) as ICondition;

							// initialize the modules
							testValue.Init(test);
							condition.Init(new Pattern(pattern, patternOptions), testValue, flags);

							// add condition to next rule that shows up
							conditions.Add(condition);
						}
						catch (Exception exc)
						{
							if (tempLogLevel >= 3)
								Manager.Log("RewriteCond: " + exc.Message, "Error");
							else
								Manager.Log("RewriteCond: " + exc, "Error");
						}
						finally
						{
							Manager.LogIf(tempLogLevel >= 3, "RewriteCond: " + match.Groups["test"].Value + " " + match.Groups["pattern"].Value + " [" + match.Groups["flags"].Value + "]", "Rule Processing");
						}

						#endregion
					}
					else if (RewriteRuleLine.IsMatch(line))
					{
						#region RewriteRule

						Match match = RewriteRuleLine.Match(line);

						string module1 = match.Groups["module1"].Value;
						string module2 = match.Groups["module2"].Value;

						Type moduleType1 = null;
						Type moduleType2 = null;

						// set the types of the first module
						if (modules.ContainsName(module1))
							moduleType1 = modules.GetModule(module1);

						// make sure the module is of the right type
						if (moduleType1 != null && moduleType1.GetInterface("IRule", false) == null)
							moduleType1 = null;

						// set the types of the second module
						if (modules.ContainsName(module2))
							moduleType2 = modules.GetModule(module2);

						// make sure the module is of the right type
						if (moduleType2 != null && moduleType2.GetInterface("IRuleAction", false) == null)
							moduleType2 = null;

						try
						{
							RegexOptions patternOptions = Manager.RuleOptions;
							IRuleFlagProcessor flags;

							if (match.Groups["flags"] != null)
								flags = SplitRuleFlags(match.Groups["flags"].Value);
							else
								flags = new RuleFlagProcessor();

							// check to see if the pattern should ignore the case when testing
							if (RuleFlagsProcessor.HasNoCase(flags))
								patternOptions |= RegexOptions.IgnoreCase;

							IRule rule = null;
							IRuleAction substitution = null;
							Pattern pattern = new Pattern(match.Groups["pattern"].Value, patternOptions);

							// create the first module
							if (moduleType1 == null)
								rule = new DefaultRule();
							else
								rule = Activator.CreateInstance(moduleType1) as IRule;

							// create the second module
							if (moduleType2 == null)
								substitution = new DefaultRuleAction();
							else
								substitution = Activator.CreateInstance(moduleType2) as IRuleAction;

							// initialize the modules
							substitution.Init(pattern, match.Groups["substitution"].Value);
							rule.Init(conditions, substitution, flags);

							// add condition to next rule that shows up
							rules.Add(rule);

							// clear conditions for next rule
							conditions.Clear();
						}
						catch (Exception exc)
						{
							if (tempLogLevel >= 3)
								Manager.Log("RewriteRule: " + exc.Message, "Error");
							else
								Manager.Log("RewriteRule: " + exc, "Error");
						}
						finally
						{
							Manager.LogIf(tempLogLevel >= 3, "RewriteRule: " + match.Groups["pattern"].Value + " " + match.Groups["substitution"].Value + " [" + match.Groups["flags"].Value + "]", "Rule Processing");
						}

						#endregion
					}
					else if (OutRewriteCondLine.IsMatch(line))
					{
						#region OutRewriteCond

						Match match = OutRewriteCondLine.Match(line);

						string module1 = match.Groups["module1"].Value;
						string module2 = match.Groups["module2"].Value;

						Type moduleType1 = null;
						Type moduleType2 = null;

						// set the types of the first module
						if (modules.ContainsName(module1))
							moduleType1 = modules.GetModule(module1);

						// make sure the module is of the right type
						if (moduleType1 != null && moduleType1.GetInterface("ICondition", false) == null)
							moduleType1 = null;

						// set the types of the second module
						if (modules.ContainsName(module2))
							moduleType2 = modules.GetModule(module2);

						// make sure the module is of the right type
						if (moduleType2 != null && moduleType2.GetInterface("IConditionTestValue", false) == null)
							moduleType2 = null;

						try
						{
							RegexOptions patternOptions = Manager.RuleOptions;
							IConditionFlagProcessor flags;

							if (match.Groups["flags"] != null)
								flags = SplitConditionFlags(match.Groups["flags"].Value);
							else
								flags = new ConditionFlagProcessor();

							// check to see if the pattern should ignore the case when testing
							if (ConditionFlagsProcessor.HasNoCase(flags))
								patternOptions |= RegexOptions.IgnoreCase;

							string test = match.Groups["test"].Value;
							string pattern = match.Groups["pattern"].Value;
							IConditionTestValue testValue = null;
							ICondition condition = null;

							// create the second module
							if (moduleType2 == null)
								testValue = GetConditionTestValue(ref test);
							else
								testValue = Activator.CreateInstance(moduleType2) as IConditionTestValue;

							// create the first module
							if (moduleType1 == null)
								condition = GetCondition(pattern);
							else
								condition = Activator.CreateInstance(moduleType1) as ICondition;

							// initialize the modules
							testValue.Init(test);
							condition.Init(new Pattern(pattern, patternOptions), testValue, flags);

							// add condition to next rule that shows up
							conditions.Add(condition);
						}
						catch (Exception exc)
						{
							if (tempLogLevel >= 3)
								Manager.Log("OutRewriteCond: " + exc.Message, "Error");
							else
								Manager.Log("OutRewriteCond: " + exc, "Error");
						}
						finally
						{
							Manager.LogIf(tempLogLevel >= 3, "OutRewriteCond: " + match.Groups["test"].Value + " " + match.Groups["pattern"].Value + " [" + match.Groups["flags"].Value + "]", "Rule Processing");
						}

						#endregion
					}
					else if (OutRewriteRuleLine.IsMatch(line))
					{
						#region OutRewriteRule

						Match match = OutRewriteRuleLine.Match(line);

						string module1 = match.Groups["module1"].Value;
						string module2 = match.Groups["module2"].Value;

						Type moduleType1 = null;
						Type moduleType2 = null;

						// set the types of the first module
						if (modules.ContainsName(module1))
							moduleType1 = modules.GetModule(module1);

						// make sure the module is of the right type
						if (moduleType1 != null && moduleType1.GetInterface("IRule", false) == null)
							moduleType1 = null;

						// set the types of the second module
						if (modules.ContainsName(module2))
							moduleType2 = modules.GetModule(module2);

						// make sure the module is of the right type
						if (moduleType2 != null && moduleType2.GetInterface("IRuleAction", false) == null)
							moduleType2 = null;

						try
						{
							RegexOptions patternOptions = Manager.RuleOptions;
							IRuleFlagProcessor flags;

							if (match.Groups["flags"] != null)
								flags = SplitRuleFlags(match.Groups["flags"].Value);
							else
								flags = new RuleFlagProcessor();

							// check to see if the pattern should ignore the case when testing
							if (RuleFlagsProcessor.HasNoCase(flags))
								patternOptions |= RegexOptions.IgnoreCase;

							IRule rule = null;
							IRuleAction substitution = null;
							Pattern pattern = new Pattern(match.Groups["pattern"].Value, patternOptions);

							// create the first module
							if (moduleType1 == null)
								rule = new DefaultRule();
							else
								rule = Activator.CreateInstance(moduleType1) as IRule;

							// create the second module
							if (moduleType2 == null)
								substitution = new DefaultOutputRuleAction();
							else
								substitution = Activator.CreateInstance(moduleType2) as IRuleAction;

							// initialize the modules
							substitution.Init(pattern, match.Groups["substitution"].Value);
							rule.Init(conditions, substitution, flags);

							// add condition to next rule that shows up
							outputRules.Add(rule);

							// clear conditions for next rule
							conditions.Clear();
						}
						catch (Exception exc)
						{
							if (tempLogLevel >= 3)
								Manager.Log("OutRewriteRule: " + exc.Message, "Error");
							else
								Manager.Log("OutRewriteRule: " + exc, "Error");
						}
						finally
						{
							Manager.LogIf(tempLogLevel >= 3, "OutRewriteRule: " + match.Groups["pattern"].Value + " " + match.Groups["substitution"].Value + " [" + match.Groups["flags"].Value + "]", "Rule Processing");
						}

						#endregion
					}
					else
					{
						unknownLines.Add(line);
					}
				}

				Manager.LogIf(tempLogLevel > 0, "Managed Fusion Rewriter Version: " + Manager.RewriterVersion, "Rule Processing");

				// clear and add new rules
				ClearRules();
				AddRules(rules);
				AddOutputRules(outputRules);

				// try to process any unknown lines
				if (unknownLines.Count > 0)
				{
					RefreshUnknownLines(ref unknownLines);

					foreach (var unknownLine in unknownLines)
						Manager.LogIf(tempLogLevel >= 4, "Not Understood: " + unknownLine, "Unknown");
				}
							

				// set the ruleset defining properties
				VirtualBase = tempBase;
				LogLocation = tempLogPath;
				LogLevel = tempLogLevel;
				EngineEnabled = tempEngineEnabled;
				Manager.LogPath = tempLogPath;
				Manager.LogEnabled = tempLogLevel > 0;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lines"></param>
		/// <returns></returns>
		protected virtual void RefreshUnknownLines(ref IList<string> lines)
		{
		}
	}
}