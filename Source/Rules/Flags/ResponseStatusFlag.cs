using System;
using System.Net;
using System.Web;
using System.Text;

namespace ManagedFusion.Rewriter.Rules.Flags
{
	public class ResponseStatusFlag : IRuleFlag
	{
		/// <summary>
		/// Tries to create a phrase string from CamelCase text.
		/// Will place spaces before capitalized letters.
		/// 
		/// Note that this method may not work for round tripping 
		/// ToCamelCase calls, since ToCamelCase strips more characters
		/// than just spaces.
		/// </summary>
		/// <param name="camelCase"></param>
		/// <returns></returns>
		public static string FromCamelCase(string camelCase)
		{
			if (camelCase == null)
				throw new ArgumentNullException("camelCase");

			StringBuilder sb = new StringBuilder(camelCase.Length + 10);
			bool first = true;
			char lastChar = '\0';

			foreach (char ch in camelCase)
			{
				if (!first && (Char.IsUpper(ch) || Char.IsDigit(ch) && !Char.IsDigit(lastChar)))
					sb.Append(' ');

				// append the character to the string builder
				sb.Append(ch);

				first = false;
				lastChar = ch;
			}

			return sb.ToString();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ResponseStatusFlag"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		public ResponseStatusFlag(string type)
		{
			SubStatusCode = 0U;
			StatusReason = null;

			uint statusCode = 0U;
			string description = null;

			try
			{
				if (UInt32.TryParse(type, out statusCode))
				{
					HttpStatusCode code = (HttpStatusCode)statusCode;
					description = FromCamelCase(code.ToString());
				}
				else
				{
					HttpStatusCode code = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), type, true);
					statusCode = (uint)code;
					description = FromCamelCase(code.ToString());
				}
			}
			catch (Exception exc)
			{
				Manager.Log("Could not create a response status from " + type, "Error");
			}

			if (statusCode < 1U || statusCode > 999U)
				throw new ArgumentOutOfRangeException("type", type, "type resulted in a status code of " + statusCode + " and it should be between 1 - 999.");

			StatusCode = statusCode;
			StatusDescription = description;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ResponseStatusFlag"/> class.
		/// </summary>
		/// <param name="statusCode">The status code.</param>
		/// <param name="subStatusCode">The sub status code.</param>
		/// <param name="reason">The reason.</param>
		/// <param name="description">The description.</param>
		public ResponseStatusFlag(uint statusCode, uint subStatusCode, string reason, string description)
		{
			if (statusCode < 1U || statusCode > 999U)
				throw new ArgumentOutOfRangeException("statusCode", statusCode, "statusCode should be between 1 - 999.");

			if (subStatusCode < 0U || subStatusCode > 999U)
				throw new ArgumentOutOfRangeException("subStatusCode", subStatusCode, "subStatusCode should be between 0 - 999.");

			StatusCode = statusCode;
			SubStatusCode = subStatusCode;

			if (String.IsNullOrEmpty(reason))
				reason = null;

			if (String.IsNullOrEmpty(description))
				description = null;

			StatusReason = reason;
			StatusDescription = description;
		}

		/// <summary>
		/// Gets or sets the status code.
		/// </summary>
		/// <value>The status code.</value>
		public uint StatusCode { get; private set; }

		/// <summary>
		/// Gets or sets the sub status code.
		/// </summary>
		/// <value>The sub status code.</value>
		public uint SubStatusCode { get; private set; }

		/// <summary>
		/// Gets or sets the status reason.
		/// </summary>
		/// <value>The status reason.</value>
		public string StatusReason { get; private set; }

		/// <summary>
		/// Gets or sets the status description.
		/// </summary>
		/// <value>The status description.</value>
		public string StatusDescription { get; private set; }

		#region IRuleFlag Members

		/// <summary>
		/// Applies the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public RuleFlagProcessorResponse Apply(RuleContext context)
		{
			if (StatusCode > 0U)
			{
				HttpResponse response = context.HttpContext.Response;

				response.StatusCode = (int)StatusCode;
				response.SubStatusCode = (int)SubStatusCode;
				response.StatusDescription = StatusReason ?? StatusDescription;

				Manager.LogIf(context.LogLevel >= 2, "HTTP Status: " + response.StatusCode + "." + response.SubStatusCode + " " + response.StatusDescription, "Rewrite");
			}

			return RuleFlagProcessorResponse.ContinueToNextFlag;
		}

		#endregion
	}
}
