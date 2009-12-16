using System.Collections.Generic;

namespace ManagedFusion.Rewriter
{
	public interface IConditionFlagProcessor : IEnumerable<IConditionFlag>
	{
		/// <summary>
		/// Adds the specified flag.
		/// </summary>
		/// <param name="flag">The flag.</param>
		void Add(IConditionFlag flag);
	}
}
