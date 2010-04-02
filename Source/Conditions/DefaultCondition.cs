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


using System.Text.RegularExpressions;

namespace ManagedFusion.Rewriter.Conditions
{
	/// <summary>
	/// 
	/// </summary>
	public class DefaultCondition : ICondition
	{
		private IConditionTestValue _test;
		private Pattern _pattern;
		private IConditionFlagProcessor _flags;

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultCondition"/> class.
		/// </summary>
		protected internal DefaultCondition() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultCondition"/> class.
		/// </summary>
		/// <param name="pattern">The pattern.</param>
		/// <param name="test">The test.</param>
		public DefaultCondition(string pattern, string test)
			: this(pattern, test, null) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultCondition"/> class.
		/// </summary>
		/// <param name="pattern">The pattern.</param>
		/// <param name="test">The test.</param>
		/// <param name="flags">The flags.</param>
		public DefaultCondition(string pattern, string test, IConditionFlagProcessor flags)
			: this(pattern, test, Manager.RuleOptions, flags) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultCondition"/> class.
		/// </summary>
		/// <param name="pattern">The pattern.</param>
		/// <param name="test">The test.</param>
		/// <param name="options">The options.</param>
		/// <param name="flags">The flags.</param>
		public DefaultCondition(string pattern, string test, RegexOptions options, IConditionFlagProcessor flags)
			: this(new Pattern(pattern, options), new DefaultConditionTestValue(test), flags) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultCondition"/> class.
		/// </summary>
		/// <param name="pattern">The pattern.</param>
		/// <param name="test">The test.</param>
		/// <param name="flags">The flags.</param>
		public DefaultCondition(Pattern pattern, IConditionTestValue test, IConditionFlagProcessor flags)
		{
			((ICondition)this).Init(pattern, test, flags);
		}

		#region ICondition Members

		/// <summary>
		/// Inits the specified text.
		/// </summary>
		/// <param name="pattern">The pattern.</param>
		/// <param name="test">The test.</param>
		/// <param name="flags">The flags.</param>
		public void Init(Pattern pattern, IConditionTestValue test, IConditionFlagProcessor flags)
		{
			_test = test;
			_flags = flags ?? new ConditionFlagProcessor();
			_pattern = pattern;
		}

		/// <summary>
		/// Gets the condition pattern.
		/// </summary>
		/// <value>The condition pattern.</value>
		public IConditionTestValue Test
		{
			get { return _test; }
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
		/// Gets the flags.
		/// </summary>
		/// <value>The flags.</value>
		public IConditionFlagProcessor Flags
		{
			get { return _flags; }
		}

		/// <summary>
		/// Determines whether the specified log level is match.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>
		/// 	<see langword="true"/> if the specified log level is match; otherwise, <see langword="false"/>.
		/// </returns>
		public bool Evaluate(ConditionContext context)
		{
			bool isMatch = false;
			string test = Test.GetValue(context);

			// pattern handles its own invert if needed
			isMatch = _pattern.IsMatch(test, context);

			Manager.LogIf(context.LogLevel >= 2, isMatch ? " Matched" : " Not Matched", context.LogCategory);
			return isMatch;
		}

		#endregion

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			return "Condition: " + Test + " - " + Pattern;
		}
	}
}
