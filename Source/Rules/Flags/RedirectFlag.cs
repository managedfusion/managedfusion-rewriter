using System;
using System.Net;

namespace ManagedFusion.Rewriter.Rules.Flags
{
	public class RedirectFlag : IRuleFlag
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RedirectFlag"/> class.
		/// </summary>
		/// <param name="statusCode">The status code.</param>
		public RedirectFlag(int statusCode)
		{
			if (statusCode >= 300U && statusCode <= 307U)
				throw new ArgumentOutOfRangeException("statusCode", statusCode, "statusCode should be between 300 - 307.");

			Type = ((int)statusCode).ToString();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RedirectFlag"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		public RedirectFlag(string type)
		{
			if (String.IsNullOrEmpty(type))
				type = "302";

			if (String.Equals(type, "permanent", StringComparison.InvariantCultureIgnoreCase))
				type = HttpStatusCode.MovedPermanently.ToString();
			else if (String.Equals(type, "temp", StringComparison.InvariantCultureIgnoreCase))
				type = HttpStatusCode.RedirectKeepVerb.ToString();

			uint statusCode = 0U;

			try
			{
				if (!UInt32.TryParse(type, out statusCode))
				{
					HttpStatusCode code = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), type, true);
					statusCode = (uint)code;
				}
			}
			catch (Exception exc)
			{
				Manager.Log("Could not create a redirect status from " + type, "Error");
			}

			if (statusCode < 300U || statusCode > 307U)
				throw new ArgumentOutOfRangeException("type", type, "type resulted in a status code of " + statusCode + " and it should be between 300 - 307.");

			Type = ((int)statusCode).ToString();
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
			Manager.LogIf(context.LogLevel >= 1, "Output: " + Type + " Redirect to " + context.SubstitutedUrl, "Rewrite");
			Manager.Redirect(context.HttpContext, Type, context.SubstitutedUrl);

			return RuleFlagProcessorResponse.ExitRuleSet;
		}

		#endregion
	}
}
