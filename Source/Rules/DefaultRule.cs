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

using System.Text.RegularExpressions;
using ManagedFusion.Rewriter.Conditions;

namespace ManagedFusion.Rewriter.Rules
{
	/// <summary>
	/// 
	/// </summary>
	public class DefaultRule : IRule
	{
		private IRuleAction _action;
		private IRuleFlagProcessor _flags;
		private IList<ICondition> _conditions;

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultRule"/> class.
		/// </summary>
		protected internal DefaultRule() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultRule"/> class.
		/// </summary>
		/// <param name="pattern">The pattern.</param>
		/// <param name="substitution">The substitution.</param>
		public DefaultRule(string pattern, string substitution)
			: this(pattern, substitution, null) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultRule"/> class.
		/// </summary>
		/// <param name="pattern">The pattern.</param>
		/// <param name="substitution">The substitution.</param>
		/// <param name="flags">The flags.</param>
		public DefaultRule(string pattern, string substitution, IRuleFlagProcessor flags)
			: this(null, pattern, substitution, Manager.RuleOptions, flags) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultRule"/> class.
		/// </summary>
		/// <param name="conditions">The conditions.</param>
		/// <param name="pattern">The pattern.</param>
		/// <param name="substitution">The substitution.</param>
		public DefaultRule(IEnumerable<ICondition> conditions, string pattern, string substitution)
			: this(conditions, pattern, substitution, null) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultRule"/> class.
		/// </summary>
		/// <param name="conditions">The conditions.</param>
		/// <param name="pattern">The pattern.</param>
		/// <param name="substitution">The substitution.</param>
		/// <param name="flags">The flags.</param>
		public DefaultRule(IEnumerable<ICondition> conditions, string pattern, string substitution, IRuleFlagProcessor flags)
			: this(conditions, pattern, substitution, Manager.RuleOptions, flags) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultRule"/> class.
		/// </summary>
		/// <param name="conditions">The conditions.</param>
		/// <param name="pattern">The pattern.</param>
		/// <param name="substitution">The substitution.</param>
		/// <param name="options">The options.</param>
		/// <param name="flags">The flags.</param>
		public DefaultRule(IEnumerable<ICondition> conditions, string pattern, string substitution, RegexOptions options, IRuleFlagProcessor flags)
			: this(conditions, new DefaultRuleAction(pattern, substitution, options), flags) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultRule"/> class.
		/// </summary>
		/// <param name="conditions">The conditions.</param>
		/// <param name="action">The action.</param>
		/// <param name="flags">The flags.</param>
		public DefaultRule(IEnumerable<ICondition> conditions, IRuleAction action, IRuleFlagProcessor flags)
		{
			((IRule)this).Init(conditions, action, flags);
		}

		#region IRule Members

		/// <summary>
		/// Inits the specified conditions.
		/// </summary>
		/// <param name="conditions">The conditions.</param>
		/// <param name="action">The action.</param>
		/// <param name="flags">The flags.</param>
		void IRule.Init(IEnumerable<ICondition> conditions, IRuleAction action, IRuleFlagProcessor flags)
		{
			_conditions = new List<ICondition>(conditions ?? new ICondition[0]);
			_action = action;
			_flags = flags ?? new RuleFlagProcessor();
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets the conditions.
		/// </summary>
		/// <value>The conditions.</value>
		public IList<ICondition> Conditions
		{
			get { return _conditions; }
		}

		/// <summary>
		/// Gets the action.
		/// </summary>
		/// <value>The action.</value>
		public IRuleAction Action
		{
			get { return _action; }
		}

		/// <summary>
		/// Gets the flags.
		/// </summary>
		/// <value>The flags.</value>
		public IRuleFlagProcessor Flags
		{
			get { return _flags; }
		}

		/// <summary>
		/// Tries the process.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public bool TryExecute(RuleContext context)
		{
			// check if this rule should be run
			if (Action.IsMatch(context))
			{
				Manager.LogIf(context.LogLevel >= 2, "Rule Pattern Matched", context.LogCategory);
				bool skipOrNext = false;
				string[] conditionValues = new string[_conditions.Count];

				// test to make sure all conditions are met
				for (int i = 0; i < _conditions.Count; i++)
				{
					ConditionContext conditionContext = new ConditionContext(i, context, _conditions[i]);
					bool containsOrNext = ConditionFlagsProcessor.HasOrNext(_conditions[i].Flags);
					bool previousContainsOrNext = ConditionFlagsProcessor.HasOrNext(_conditions[Math.Max(0, i - 1)].Flags);

					if (skipOrNext && (previousContainsOrNext || containsOrNext))
						continue;
					else
						skipOrNext = false;

					// test the condition if it fails and then terminate
					if (!_conditions[i].Evaluate(conditionContext))
					{
						if (containsOrNext)
							continue;
						else
							return false;
					}
					else if (containsOrNext)
						skipOrNext = true;
				}

				// call an external method that might want to inherit from this rule implimentation
				OnExecuting(context);

				// process the substition for the pattern
				Action.Execute(context);

				// the pattern matched
				return true;
			}

			// the pattern did not match
			return false;
		}

		#endregion

		/// <summary>
		/// Called when [executing].
		/// </summary>
		/// <param name="context">The context.</param>
		protected virtual void OnExecuting(RuleContext context)
		{
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </returns>
		public override string ToString()
		{
			return "Rule: " + Action + ", " + Conditions.Count + " Conditions";
		}
	}
}
