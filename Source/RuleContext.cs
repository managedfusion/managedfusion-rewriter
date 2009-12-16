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
using System.Collections.Generic;

namespace ManagedFusion.Rewriter
{
	/// <summary>
	/// 
	/// </summary>
	public class RuleContext : RuleSetContext
	{
		private Uri _substitutedUrl;
		private byte[] _substitutedContent;
		private byte[] _currentContent;

		/// <summary>
		/// Initializes a new instance of the <see cref="RuleContext"/> class.
		/// </summary>
		/// <param name="copy">The copy.</param>
		internal RuleContext(RuleContext copy)
			: base(copy)
		{
			RuleIndex = copy.RuleIndex;
			LogCategory = copy.LogCategory;
			_currentContent = copy._currentContent;
			_substitutedContent = copy.SubstitutedContent;
			CurrentUrl = copy.CurrentUrl;
			_substitutedUrl = copy.SubstitutedUrl;
			CurrentRule = copy.CurrentRule;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RuleContext"/> class.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="ruleSetContext">The rule set context.</param>
		/// <param name="currentUrl">The current URL.</param>
		/// <param name="rule">The rule.</param>
		public RuleContext(int index, RuleSetContext ruleSetContext, Uri currentUrl, IRule rule)
			: base(ruleSetContext)
		{
			if (currentUrl == null)
				throw new ArgumentNullException("currentUrl");

			if (rule == null)
				throw new ArgumentNullException("rule");

			RuleIndex = index;
			LogCategory = "Rule " + index;
			_currentContent = null;
			_substitutedContent = null;
			CurrentUrl = currentUrl;
			_substitutedUrl = currentUrl;
			CurrentRule = rule;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RuleContext"/> class.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="ruleSetContext">The rule set context.</param>
		/// <param name="currentUrl">The current content.</param>
		/// <param name="rule">The rule.</param>
		public RuleContext(int index, RuleSetContext ruleSetContext, byte[] currentContent, IRule rule)
			: base(ruleSetContext)
		{
			if (currentContent == null)
				throw new ArgumentNullException("currentContent");

			if (rule == null)
				throw new ArgumentNullException("rule");

			RuleIndex = index;
			LogCategory = "Rule " + index;
			_currentContent = currentContent;
			_substitutedContent = currentContent;
			CurrentUrl = ruleSetContext.RequestedUrl;
			_substitutedUrl = ruleSetContext.RequestedUrl;
			CurrentRule = rule;
		}

		/// <summary>
		/// Gets or sets the index.
		/// </summary>
		/// <value>The index.</value>
		public int? RuleIndex { get; set; }

		/// <summary>
		/// Gets or sets the current URL.
		/// </summary>
		/// <value>The current URL.</value>
		public Uri CurrentUrl { get; private set; }

		/// <summary>
		/// Gets or sets the current content.
		/// </summary>
		public byte[] CurrentContent
		{
			get
			{
				byte[] output = new byte[_currentContent.LongLength];
				Array.Copy(_currentContent, output, _currentContent.LongLength);
				return output;
			}
		}

		/// <summary>
		/// Gets or sets the substituted URL.
		/// </summary>
		/// <value>The substituted URL.</value>
		public Uri SubstitutedUrl
		{
			get { return _substitutedUrl; }
			set
			{
				if (IsOutputRuleSet)
					throw new NotSupportedException("You cannot substitute the URL on an output rule.");

				if (value == null)
					throw new ArgumentNullException("value");

				_substitutedUrl = value;
			}
		}

		/// <summary>
		/// Gets or sets the substituted content.
		/// </summary>
		public byte[] SubstitutedContent
		{
			get { return _substitutedContent; }
			set
			{
				if (!IsOutputRuleSet)
					throw new NotSupportedException("You cannot substitute the content on an input rule.");

				if (value == null)
					throw new ArgumentNullException("value");

				_substitutedContent = value;
			}
		}

		/// <summary>
		/// Gets or sets the current rule.
		/// </summary>
		/// <value>The current rule.</value>
		public IRule CurrentRule { get; private set; }

		/// <summary>
		/// Gets the flags.
		/// </summary>
		/// <value>The flags.</value>
		public virtual IRuleFlagProcessor RuleFlags
		{
			get { return CurrentRule.Flags; }
		}

		/// <summary>
		/// Gets or sets the conditions.
		/// </summary>
		/// <value>The conditions.</value>
		public virtual IList<ICondition> Conditions
		{
			get { return CurrentRule.Conditions; }
		}

		/// <summary>
		/// Gets the condition value.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public virtual string GetConditionValue(int index)
		{
			if (Conditions.Count - 1 < index)
				return null;

			ICondition condition = Conditions[index];

			if (condition == null)
				return null;

			return condition.Test.GetValue(new ConditionContext(index, this, condition));
		}

		/// <summary>
		/// Gets the relative input URL.
		/// </summary>
		/// <returns></returns>
		public virtual string GetRelativeInputUrl()
		{
			return RequestedUrl.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);
		}
	}
}