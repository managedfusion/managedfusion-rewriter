
using System.Web;

namespace ManagedFusion.Rewriter.Rules.Flags
{
	public class ResponseMimeTypeFlag : IRuleFlag
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ResponseStatusFlag"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		public ResponseMimeTypeFlag(string type)
		{
			Type = type;
		}

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		public string Type { get; private set; }

		#region IRuleFlag Members

		/// <summary>
		/// Applies the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public RuleFlagProcessorResponse Apply(RuleContext context)
		{
			SetMimeType(context.HttpContext, Type);

			Manager.LogIf(context.LogLevel >= 2, "Content Type: " + Type, "Rewrite");
			return RuleFlagProcessorResponse.ContinueToNextFlag;
		}

		#endregion

		/// <summary>
		/// Sets the type of the MIME.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="type">The type.</param>
		private void SetMimeType(HttpContext context, string type)
		{
			context.Response.ContentType = type;
		}
	}
}