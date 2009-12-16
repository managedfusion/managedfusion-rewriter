using System;

namespace ManagedFusion.Rewriter.Rules.Flags
{
	public class QueryStringAppendFlag : IRuleFlag
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="QueryStringAppendFlag"/> class.
		/// </summary>
		public QueryStringAppendFlag()
		{
		}

		#region IRuleFlag Members

		/// <summary>
		/// Applies the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public RuleFlagProcessorResponse Apply(RuleContext context)
		{
			context.SubstitutedUrl = AppendQueryString(context.SubstitutedUrl, context.CurrentUrl);

			return RuleFlagProcessorResponse.ContinueToNextFlag;
		}

		#endregion

		/// <summary>
		/// Appends the query string.
		/// </summary>
		/// <param name="substituedUrl">The substitued URL.</param>
		/// <param name="existingUrl">The existing URL.</param>
		/// <returns></returns>
		private Uri AppendQueryString(Uri substituedUrl, Uri existingUrl)
		{
			string append = existingUrl.Query.TrimStart('?');

			if (!String.IsNullOrEmpty(append))
			{
				UriBuilder builder = new UriBuilder(substituedUrl);

				if (String.IsNullOrEmpty(builder.Query) || builder.Query == "?")
					builder.Query = append;
				else
					builder.Query = builder.Query.TrimStart('?') + "&" + append;

				return builder.Uri;
			}

			return substituedUrl;
		}
	}
}
