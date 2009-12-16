using System;

namespace ManagedFusion.Rewriter.Rules.Flags
{
	public class ServerVariableFlag : IRuleFlag
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ServerVariableFlag"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		/// <param name="replace">if set to <c>true</c> [replace].</param>
		public ServerVariableFlag(string name, string value, bool replace)
		{
			Name = name;
			Value = value;
			Replace = replace;
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; private set; }

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <value>The value.</value>
		public string Value { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="ServerVariableFlag"/> is replace.
		/// </summary>
		/// <value><c>true</c> if replace; otherwise, <c>false</c>.</value>
		public bool Replace { get; private set; }

		#region IRuleFlag Members

		/// <summary>
		/// Applies the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public RuleFlagProcessorResponse Apply(RuleContext context)
		{
			Manager.SetServerVariable(context.HttpContext, Name, Value, Replace);

			Manager.LogIf(context.LogLevel >= 2, "Set Server Variable: " + Name, "Rewrite");
			return RuleFlagProcessorResponse.ContinueToNextFlag;
		}

		#endregion
	}
}
