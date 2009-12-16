using System;

using System.Web;

namespace ManagedFusion.Rewriter.Rules.Flags
{
	public class ResponseCookieFlag : IRuleFlag
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ResponseCookieFlag"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		/// <param name="domain">The domain.</param>
		/// <param name="expires">The expires.</param>
		/// <param name="path">The path.</param>
		public ResponseCookieFlag(string name, string value, string domain, TimeSpan? expires, string path)
		{
			Name = name;
			Value = value;
			Domain = domain;
			Expires = expires;
			Path = path;
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; private set; }

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public string Value { get; private set; }

		/// <summary>
		/// Gets or sets the domain.
		/// </summary>
		/// <value>The domain.</value>
		public string Domain { get; private set; }

		/// <summary>
		/// Gets or sets the expires.
		/// </summary>
		/// <value>The expires.</value>
		public TimeSpan? Expires { get; private set; }

		/// <summary>
		/// Gets or sets the path.
		/// </summary>
		/// <value>The path.</value>
		public string Path { get; private set; }

		#region IRuleFlag Members

		/// <summary>
		/// Applies the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public RuleFlagProcessorResponse Apply(RuleContext context)
		{
			var httpContext = context.HttpContext;

			HttpCookie cookie = new HttpCookie(Name, Value);
			cookie.Domain = Domain;

			if (Expires.HasValue)
				cookie.Expires = (DateTime.Now + Expires.Value);

			if (String.IsNullOrEmpty(Path))
				cookie.Path = Path;

			httpContext.Response.Cookies.Add(cookie);

			Manager.LogIf(context.LogLevel >= 2, "Cookie: " + Name + ":" + Value, "Rewrite");
			return RuleFlagProcessorResponse.ContinueToNextFlag;
		}

		#endregion
	}
}
