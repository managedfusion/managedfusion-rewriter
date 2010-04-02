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

using System.Collections.Generic;
using ManagedFusion.Rewriter.Conditions.Flags;

namespace ManagedFusion.Rewriter.Conditions
{
	/// <summary>
	/// 
	/// </summary>
	public class ConditionFlagProcessor : IConditionFlagProcessor
	{
		private List<IConditionFlag> _flags;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConditionFlagProcessor"/> class.
		/// </summary>
		public ConditionFlagProcessor()
		{
			_flags = new List<IConditionFlag>(0);
		}

		#region IConditionFlagProcessor Members

		/// <summary>
		/// Adds the specified flag.
		/// </summary>
		/// <param name="flag">The flag.</param>
		public void Add(IConditionFlag flag)
		{
			_flags.Add(flag);
		}

		#endregion

		#region IEnumerable<IConditionFlag> Members

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<IConditionFlag> GetEnumerator()
		{
			return _flags.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _flags.GetEnumerator();
		}

		#endregion
	}

	/// <summary>
	/// 
	/// </summary>
	public static class ConditionFlagsProcessor
	{
		/// <summary>
		/// Determines whether [has no case] [the specified flags].
		/// </summary>
		/// <param name="flags">The flags.</param>
		/// <returns>
		/// 	<see langword="true"/> if [has no case] [the specified flags]; otherwise, <see langword="false"/>.
		/// </returns>
		public static bool HasNoCase(IConditionFlagProcessor flags)
		{
			foreach (IConditionFlag flag in flags)
				if (flag is NoCaseFlag)
					return true;

			return false;
		}

		/// <summary>
		/// Determines whether [has or next] [the specified flags].
		/// </summary>
		/// <param name="flags">The flags.</param>
		/// <returns>
		/// 	<see langword="true"/> if [has or next] [the specified flags]; otherwise, <see langword="false"/>.
		/// </returns>
		public static bool HasOrNext(IConditionFlagProcessor flags)
		{
			foreach (IConditionFlag flag in flags)
				if (flag is OrNextFlag)
					return true;

			return false;
		}
	}
}