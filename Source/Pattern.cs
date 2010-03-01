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
using System.Text.RegularExpressions;

namespace ManagedFusion.Rewriter
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class Pattern
	{
		private static readonly RegexOptions FileOptions = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant;
		private static readonly Regex Variables = new Regex(@"(?<servervar>%{[\s]*((?<type>[\w\-]+)[\s]*:)?[\s]*(?<name>[\w\-]+)[\s]*})|(?<condvar>%(?<index>[1-9]+))|(?<rulevar>\$(?<index>[1-9]+))", FileOptions);

		private readonly Regex _pattern;
		private readonly bool _invertMatch;
		private int? _groupCount;

		/// <summary>
		/// Initializes a new instance of the <see cref="Pattern"/> class.
		/// </summary>
		/// <param name="pattern">The pattern.</param>
		/// <param name="options">The options.</param>
		public Pattern(string pattern, RegexOptions options)
		{
			_invertMatch = false;

			if (pattern[0] == '!')
			{
				_invertMatch = true;
				pattern = pattern.Substring(1);
			}

			// replace a common use scenario from apache with the correct representation
			if (pattern == ".")
				pattern = "^(.*)$";

			_pattern = new Regex(pattern, options);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Pattern"/> class.
		/// </summary>
		/// <param name="pattern">The pattern.</param>
		/// <param name="invertMatch">if set to <see langword="true"/> [invert match].</param>
		/// <param name="options">The options.</param>
		public Pattern(string pattern, bool invertMatch, RegexOptions options)
		{
			_invertMatch = invertMatch;
			_pattern = new Regex(pattern, options);
		}

		/// <summary>
		/// Gets a value indicating whether [invert match].
		/// </summary>
		/// <value><c>true</c> if [invert match]; otherwise, <c>false</c>.</value>
		public bool InvertMatch { get { return _invertMatch; } }

		/// <summary>
		/// Determines whether the specified input is match.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns>
		/// 	<see langword="true"/> if the specified input is match; otherwise, <see langword="false"/>.
		/// </returns>
		public bool IsMatch(string input)
		{
			if (_invertMatch)
				return !_pattern.IsMatch(input);

			return _pattern.IsMatch(input);
		}

		/// <summary>
		/// Determines whether the specified input is match.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <param name="context">The context.</param>
		/// <returns>
		/// 	<see langword="true"/> if the specified input is match; otherwise, <see langword="false"/>.
		/// </returns>
		public bool IsMatch(string input, RuleContext context)
		{
			// if the input contains a rule or a condition placeholders that need to be 
			// replaced from the input URL it needs to be processed by the rule pattern 
			// before we can check if it is a match
			if (input.IndexOfAny(new char[] { '%', '$' }) >= 0)
				input = Replace(input, context);

			return IsMatch(input);
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <param name="index">The index.</param>
		/// <returns>Returns the value of the pattern for the <paramref name="index"/></returns>
		public string GetValue(string input, int index)
		{
			// if this is an invert match and we are getting the value for a specific grouping
			// then we have to return the whole value, because there is no current way to find
			// the group value of an invert match
			if (InvertMatch)
				return input;

			var match = _pattern.Match(input);
			if (match.Success)
			{
				var group = match.Groups[index];
				if (group.Success)
					return group.Value;
			}

			return String.Empty;
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <param name="index">The index.</param>
		/// <param name="context">The context.</param>
		/// <returns>Returns the value of the pattern for the <paramref name="index"/></returns>
		public string GetValue(string input, int index, RuleContext context)
		{
			return GetValue(input, index);
		}

		/// <summary>
		/// Gets the value of a condition.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="index"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public string GetValue(string input, int index, ConditionContext context)
		{
			return GetValue(input, index);
		}

		/// <summary>
		/// Gets the group count.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns>Returns the count of the pattern.</returns>
		public int GetGroupCount(string input)
		{
			if (!_groupCount.HasValue)
			{
				// if this is an invert match and we are getting the group count then we
				// have to return only one group, because there is no current way to find
				// the group value of an invert match, so the whole thing is assumed to be
				// the group for this pattern
				if (InvertMatch)
					_groupCount = 1;
				else
					_groupCount = _pattern.GetGroupNumbers().Length;
			}

			return _groupCount.Value;
		}

		/// <summary>
		/// Replaces the specified input.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <param name="replacement">The replacement.</param>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public static string Replace(string input, RuleContext context)
		{
			string replacement = input;

			if (context != null)
				replacement = Variables.Replace(replacement, match => {
					if (match.Groups["rulevar"].Success)
					{
						var ruleVar = GetRuleVariable(match);
						return ruleVar.GetValue(input, context);
					}
					else if (match.Groups["condvar"].Success)
					{
						var condVar = GetConditionVariable(match);
						return condVar.GetValue(input, context);
					}
					else if (match.Groups["servervar"].Success)
					{
						var serverVar = GetServerVariable(match);
						return serverVar.GetValue(input, context);
					}
					return match.Value;
				});

			return replacement;
		}

		/// <summary>
		/// Replaces the specified input for a rule variable.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <param name="replacement">The replacement.</param>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public string Replace(string input, string replacement, RuleContext context)
		{
			if (context != null)
				replacement = Variables.Replace(replacement, match => {
					if (match.Groups["condvar"].Success)
					{
						var condVar = GetConditionVariable(match);
						return condVar.GetValue(input, context);
					}
					else if (match.Groups["servervar"].Success)
					{
						var serverVar = GetServerVariable(match);
						return serverVar.GetValue(input, context);
					}
					return match.Value;
				});


			return _pattern.Replace(input, replacement);
		}

		/// <summary>
		/// Gets the rule variable.
		/// </summary>
		/// <param name="testMatch">The test.</param>
		/// <returns></returns>
		private static RuleVariable GetRuleVariable(Match testMatch)
		{
			if (testMatch == null)
				throw new ArgumentNullException("testMatch");

			if (!testMatch.Success)
				throw new ArgumentException("Match does not match and a Rule Variable cannot be created.", "testMatch");

			string tempIndex = testMatch.Groups["index"].Value;

			return new RuleVariable(Convert.ToInt32(tempIndex));
		}

		/// <summary>
		/// Gets the condition variable.
		/// </summary>
		/// <param name="testMatch">The test.</param>
		/// <returns></returns>
		private static ConditionVariable GetConditionVariable(Match testMatch)
		{
			if (testMatch == null)
				throw new ArgumentNullException("testMatch");

			if (!testMatch.Success)
				throw new ArgumentException("Match does not match and a Condition Variable cannot be created.", "testMatch");

			string tempIndex = testMatch.Groups["index"].Value;

			return new ConditionVariable(Convert.ToInt32(tempIndex));
		}

		/// <summary>
		/// Gets the server variable.
		/// </summary>
		/// <param name="testMatch">The test.</param>
		/// <returns></returns>
		private static ServerVariable GetServerVariable(Match testMatch)
		{
			if (testMatch == null)
				throw new ArgumentNullException("testMatch");

			if (!testMatch.Success)
				throw new ArgumentException("Match does not match and a Server Variable cannot be created.", "testMatch");

			ServerVariableType type;

			string tempType = testMatch.Groups["type"].Value;
			string tempName = testMatch.Groups["name"].Value;

			switch (tempType.ToLower())
			{
				case "header":
				case "http":
					type = ServerVariableType.Headers;
					break;
				case "cookie":
					type = ServerVariableType.Cookies;
					break;
				case "form":
				case "post":
					type = ServerVariableType.Form;
					break;
				case "query":
				case "get":
					type = ServerVariableType.QueryString;
					break;
				case "var":
				case "server":
				default:
					type = ServerVariableType.ServerVariables;
					break;
			}

			return new ServerVariable(tempName, type);
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			return ToString(false);
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <param name="showOnlyPattern">if set to <see langword="true"/> [show only pattern].</param>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public string ToString(bool showOnlyPattern)
		{
			return (!showOnlyPattern && _invertMatch ? "!" : String.Empty) + _pattern;
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public string ToString(RuleContext context)
		{
			return Replace(ToString(false), context);
		}
	}
}
