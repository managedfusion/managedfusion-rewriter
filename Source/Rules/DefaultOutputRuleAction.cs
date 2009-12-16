/** 
 * Copyright (C) 2007-2008 Nicholas Berardi, Managed Fusion, LLC (nick@managedfusion.com)
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

namespace ManagedFusion.Rewriter.Rules
{
	/// <summary>
	/// 
	/// </summary>
	public class DefaultOutputRuleAction : IRuleAction
	{
		private Pattern _pattern;
		private string _substitution;

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultOutputRuleAction"/> class.
		/// </summary>
		protected internal DefaultOutputRuleAction() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultOutputRuleAction"/> class.
		/// </summary>
		/// <param name="pattern">The pattern.</param>
		/// <param name="substitution">The substitution.</param>
		public DefaultOutputRuleAction(string pattern, string substitution)
			: this(pattern, substitution, Manager.RuleOptions) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultOutputRuleAction"/> class.
		/// </summary>
		/// <param name="pattern">The pattern.</param>
		/// <param name="substitution">The substitution.</param>
		/// <param name="options">The options.</param>
		public DefaultOutputRuleAction(string pattern, string substitution, RegexOptions options)
			: this(new Pattern(pattern, options), substitution) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultOutputRuleAction"/> class.
		/// </summary>
		/// <param name="pattern">The pattern.</param>
		/// <param name="text">The text.</param>
		public DefaultOutputRuleAction(Pattern pattern, string substitution)
		{
			((IRuleAction)this).Init(pattern, substitution);
		}

		#region IRuleSubstitution Members

		/// <summary>
		/// Inits the specified text.
		/// </summary>
		/// <param name="pattern">The pattern.</param>
		/// <param name="text">The text.</param>
		void IRuleAction.Init(Pattern pattern, string substitution)
		{
			_pattern = pattern;
			_substitution = substitution;
		}

		/// <summary>
		/// Gets the pattern.
		/// </summary>
		/// <value>The pattern.</value>
		public Pattern Pattern
		{
			get { return _pattern; }
		}

		/// <summary>
		/// Gets the text.
		/// </summary>
		/// <value>The text.</value>
		public string Substitution
		{
			get { return _substitution; }
		}

		/// <summary>
		/// Checks if the pattern is a match to the context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public virtual bool IsMatch(RuleContext context)
		{
			string content = System.Text.Encoding.UTF8.GetString(context.CurrentContent);
			//Manager.LogIf(context.LogLevel >= 2, "Input: " + testUrl, context.LogCategory);

			return Pattern.IsMatch(content);
		}

		/// <summary>
		/// Processes the specified log level.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public virtual void Execute(RuleContext context)
		{
			string content = System.Text.Encoding.UTF8.GetString(context.CurrentContent);
			string substituedContent = Pattern.Replace(content, _substitution, context);

			//Manager.LogIf(context.LogLevel >= 2, "Output: " + substituedUrl, context.LogCategory);
			context.SubstitutedContent = System.Text.Encoding.UTF8.GetBytes(substituedContent);
		}

		#endregion

		/// <summary>
		/// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </returns>
		public override string ToString()
		{
			return Pattern + " - " + Substitution;
		}
	}
}
