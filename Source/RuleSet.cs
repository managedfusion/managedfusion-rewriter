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

using System.IO;
using System.Web;
using ManagedFusion.Rewriter.Rules;
using System.Web.Hosting;
using System.Security.Permissions;

namespace ManagedFusion.Rewriter
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class RuleSet
	{
		private static readonly string DefaultLogFileName;

		/// <summary>
		/// Initializes the <see cref="RuleSet"/> class.
		/// </summary>
		static RuleSet()
		{
			DefaultLogFileName = "url-rewrite-log.txt";
		}

		private IList<IRule> _rules;
		private IList<IRule> _outputRules;

		/// <summary>
		/// Initializes a new instance of the <see cref="RuleSet"/> class.
		/// </summary>
		public RuleSet()
		{
			// create list of rules
			_rules = new List<IRule>();
			_outputRules = new List<IRule>();

			// the defaults for hte properties
			MaxInternalTransfers = 10;
		}

		/// <summary>
		/// Gets or sets a value indicating whether [engine enabled].
		/// </summary>
		/// <value>
		/// 	<see langword="true"/> if [engine enabled]; otherwise, <see langword="false"/>.
		/// </value>
		public bool EngineEnabled
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets or sets the log level.
		/// </summary>
		/// <value>The log level.</value>
		public int LogLevel
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets or sets the log location.
		/// </summary>
		/// <value>The log location.</value>
		public string LogLocation
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets or set the physical base.
		/// </summary>
		public string PhysicalBase
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets or sets the base.
		/// </summary>
		/// <value>The base.</value>
		public string VirtualBase
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets or sets the max internal transfers allowed.
		/// </summary>
		/// <remarks>In order to prevent endless loops of internal transfers issued by rewrite requests in IIS7 or above, the ruleset 
		/// aborts the request after reaching a maximum number of such redirects and responds with an 500 Internal Server Error. 
		/// If you really need more internal redirects than 10 per request, you may increase the default to the desired value.</remarks>
		public int MaxInternalTransfers
		{
			get;
			protected set;
		}

		/// <summary>
		/// Adds the rule.
		/// </summary>
		/// <param name="rule">The rule.</param>
		public void AddRule(IRule rule)
		{
			_rules.Add(rule);
		}

		/// <summary>
		/// Adds the output rule.
		/// </summary>
		/// <param name="rule">The rule.</param>
		public void AddOutputRule(IRule rule)
		{
			_outputRules.Add(rule);
		}

		/// <summary>
		/// Adds the rules.
		/// </summary>
		/// <param name="rules">The rules.</param>
		public void AddRules(IEnumerable<IRule> rules)
		{
			foreach (IRule rule in rules)
				AddRule(rule);
		}

		/// <summary>
		/// Adds the output rules.
		/// </summary>
		/// <param name="rules">The rules.</param>
		public void AddOutputRules(IEnumerable<IRule> rules)
		{
			foreach (IRule rule in rules)
				AddOutputRule(rule);
		}

		/// <summary>
		/// Clears the rules.
		/// </summary>
		public void ClearRules()
		{
			_rules.Clear();
			_outputRules.Clear();
		}

		/// <summary>
		/// Gets the rule count.
		/// </summary>
		public int RuleCount
		{
			get { return _rules.Count; }
		}

		/// <summary>
		/// Gets the output rule count.
		/// </summary>
		public int OutputRuleCount
		{
			get { return _outputRules.Count; }
		}

		#region Refresh Actions

		/// <summary>
		/// Normalizes the log location.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns></returns>
		protected string NormalizeLogLocation(string path)
		{
			if (String.IsNullOrEmpty(path))
				throw new ArgumentNullException("path");

			string pathRoot = Path.GetPathRoot(path);

			// if the path root is empty then add a root to it
			if (String.IsNullOrEmpty(pathRoot))
			{
				pathRoot = Path.DirectorySeparatorChar.ToString();
				path = pathRoot + path;
			}

			// if the log path is not rooted to the drive then we need to path the path according to 
			// our web application root directory
			if (pathRoot == Path.DirectorySeparatorChar.ToString())
				path = HostingEnvironment.MapPath(path);

			// if the path is a directory then add our default filename to it
			if (String.IsNullOrEmpty(Path.GetFileName(path)))
				path = Path.Combine(path, DefaultLogFileName);

			// normalize the path in case anything wacky is in there
			path = Path.GetFullPath(path);

			// verify write access is granted to the path
			new FileIOPermission(FileIOPermissionAccess.Write, path).Demand();

			return path;
		}

		#endregion

		#region Run Actions

		/// <summary>
		/// Determines whether [is internal transfer] [the specified context].
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>
		/// 	<see langword="true"/> if [is internal transfer] [the specified context]; otherwise, <see langword="false"/>.
		/// </returns>
		private bool IsInternalTransfer(HttpContext context)
		{
			return !String.IsNullOrEmpty(context.Request.Headers["X-Rewriter-Transfer"]);
		}

		/// <summary>
		/// Gets the total count of internal transfers that have occured.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>Returns the count of internal transfers that have occured.</returns>
		private int InternalTransferCount(HttpContext context)
		{
			int transferCount;
			string transferCountHeader = context.Request.Headers["X-Rewriter-Transfer"];

			if (String.IsNullOrEmpty(transferCountHeader) || !Int32.TryParse(transferCountHeader, out transferCount))
				transferCount = 0;

			return transferCount;
		}

		/// <summary>
		/// Removes the base.
		/// </summary>
		/// <param name="baseFrom">The base from.</param>
		/// <param name="url">The URL.</param>
		/// <returns></returns>
		private Uri RemoveBase(string baseFrom, Uri url)
		{
			string urlPath = url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped);

			if (urlPath.StartsWith(baseFrom))
				urlPath = urlPath.Remove(0, baseFrom.Length);

			if (!urlPath.StartsWith("/"))
				urlPath = "/" + urlPath;

			return new Uri(url, urlPath);
		}

		/// <summary>
		/// Adds the base.
		/// </summary>
		/// <param name="baseFrom">The base from.</param>
		/// <param name="url">The URL.</param>
		/// <returns></returns>
		private Uri AddBase(string baseFrom, Uri url)
		{
			string urlPath = url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped);

			if (!urlPath.StartsWith(baseFrom))
				urlPath = baseFrom + urlPath;

			while (urlPath.Contains("//"))
				urlPath = urlPath.Replace("//", "/");

			return new Uri(url, urlPath);
		}

		/// <summary>
		/// Runs the rules.
		/// </summary>
		/// <param name="httpContext">The HTTP context.</param>
		/// <param name="url">The URL.</param>
		/// <returns>
		/// Returns a rewritten <see cref="System.Uri"/>, or a value of <see langword="null"/> if no rewriting was done to <paramref name="url"/>.
		/// </returns>
		public Uri RunRules(HttpContext httpContext, Uri url)
		{
			RuleSetContext context = new RuleSetContext(this, url, httpContext);
			Uri currentUrl = url;

			if (!EngineEnabled)
				Manager.LogIf(!EngineEnabled && LogLevel >= 9, "Rewrite Engine Is DISABLED", "Rewrite");

			if (_rules.Count > 0 && EngineEnabled)
			{
				Manager.LogIf(LogLevel >= 1, "**********************************************************************************");
				Manager.LogIf(LogLevel >= 1, "Input: " + currentUrl, "Rewrite");

				// check if max number of internal transfers have been exceeded
				if (InternalTransferCount(httpContext) > MaxInternalTransfers)
				{
					string message = "Exceeded the max number of internal transfers.";
					Manager.LogIf(LogLevel >= 1, message, "Error");
					throw new HttpException(500, message);
				}

				IRuleFlagProcessor temporyFlags = null;
				bool skipNextChain = false;
				Uri initialUrl = currentUrl;

				if (!String.IsNullOrEmpty(VirtualBase) && VirtualBase != "/")
					currentUrl = RemoveBase(VirtualBase, currentUrl);

				// process rules according to their settings
				for (int i = 0; i < _rules.Count; i++)
				{
					RuleContext ruleContext = new RuleContext(i, context, currentUrl, _rules[i]);
					temporyFlags = _rules[i].Flags;

					// continue if this rule shouldn't be processed because it doesn't allow internal transfer requests
					if (RuleFlagsProcessor.HasNotForInternalSubRequests(temporyFlags) && IsInternalTransfer(httpContext))
						continue;

					bool containsChain = RuleFlagsProcessor.HasChain(_rules[i].Flags);
					bool previousContainsChain = RuleFlagsProcessor.HasChain(_rules[Math.Max(0, i - 1)].Flags);

					// if the previous rule doesn't contain a chain flag then set the initial URL
					// this will be used to reset a chain if one of the chain rules fail
					if (!previousContainsChain)
						initialUrl = currentUrl;

					// skip if the current rule or the last rule has a chain flag
					// and if the skip next chain is set
					if (skipNextChain && (previousContainsChain || containsChain))
						continue;
					else
						skipNextChain = false;

					if (_rules[i].TryExecute(ruleContext))
					{
						var flagResponse = temporyFlags.Apply(ruleContext);
						currentUrl = ruleContext.SubstitutedUrl;
						i = ruleContext.RuleIndex ?? -1;
						bool breakLoop = false;

						// apply the flags to the rules, and only do special processing
						// for the flag responses listed in the switch statement below
						switch (flagResponse)
						{
							case RuleFlagProcessorResponse.ExitRuleSet:
								return null;
							case RuleFlagProcessorResponse.LastRule:
								breakLoop = true;
								break;
						}

						// break the loop because we have reached the last rule as indicated by a flag
						if (breakLoop)
							break;
					}
					else if (containsChain)
					{
						skipNextChain = true;

						// reset the current URL back to the initial URL from the start of the chain
						currentUrl = initialUrl;
					}
					else if (previousContainsChain)
					{
						// reset the current URL back to the initial URL from the start of the chain
						currentUrl = initialUrl;
					}
				}

				// if the scheme, host, and ports do not match on the request vs the rewrite a redirect needs to be performed instead of a rewrite
				if (Uri.Compare(currentUrl, context.RequestedUrl, UriComponents.SchemeAndServer, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) != 0)
				{
					Manager.LogIf(LogLevel >= 1, "Output: 302 Redirect to " + currentUrl, "Rewrite");
					Manager.Redirect(httpContext, "found", currentUrl);
				}

				if (!String.IsNullOrEmpty(VirtualBase) && VirtualBase != "/")
					currentUrl = AddBase(VirtualBase, currentUrl);

				Manager.LogIf(LogLevel >= 1, "Output: " + currentUrl, "Rewrite");
				Manager.LogIf(LogLevel >= 1, "**********************************************************************************");

				Manager.TryToAddXRewriteUrlHeader(httpContext);
				Manager.TryToAddVanityHeader(httpContext);
			}

			// if the http request url matches for both the request and the rewrite no work was done so the url should be null
			if (Uri.Compare(currentUrl, url, UriComponents.HttpRequestUrl, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) == 0)
				currentUrl = null;

			return currentUrl;
		}

		#endregion

		#region Run Output Actions

		/// <summary>
		/// 
		/// </summary>
		/// <param name="httpContext"></param>
		/// <param name="content"></param>
		/// <returns></returns>
		public byte[] RunOutputRules(HttpContext httpContext, byte[] content)
		{
			RuleSetContext context = new RuleSetContext(this, content, httpContext);
			byte[] currentContent = content;

			if (!EngineEnabled)
				Manager.LogIf(!EngineEnabled && LogLevel >= 9, "Rewrite Engine Is DISABLED", "OutRewrite");

			if (_outputRules.Count > 0 && EngineEnabled)
			{
				Manager.LogIf(LogLevel >= 1, "**********************************************************************************");
				//Manager.LogIf(LogLevel >= 9, "Input: " + currentContent, "OutRewrite");

				IRuleFlagProcessor temporyFlags = null;
				bool skipNextChain = false;
				byte[] initialContent = currentContent;

				// process rules according to their settings
				for (int i = 0; i < _outputRules.Count; i++)
				{
					RuleContext ruleContext = new RuleContext(i, context, currentContent, _outputRules[i]);
					temporyFlags = _outputRules[i].Flags;

					bool containsChain = RuleFlagsProcessor.HasChain(_outputRules[i].Flags);
					bool previousContainsChain = RuleFlagsProcessor.HasChain(_outputRules[Math.Max(0, i - 1)].Flags);

					// if the previous rule doesn't contain a chain flag then set the initial URL
					// this will be used to reset a chain if one of the chain rules fail
					if (!previousContainsChain)
						initialContent = currentContent;

					// skip if the current rule or the last rule has a chain flag
					// and if the skip next chain is set
					if (skipNextChain && (previousContainsChain || containsChain))
						continue;
					else
						skipNextChain = false;

					if (_outputRules[i].TryExecute(ruleContext))
					{
						var flagResponse = temporyFlags.Apply(ruleContext);
						currentContent = ruleContext.SubstitutedContent;
						i = ruleContext.RuleIndex ?? -1;
						bool breakLoop = false;

						// apply the flags to the rules, and only do special processing
						// for the flag responses listed in the switch statement below
						switch (flagResponse)
						{
							case RuleFlagProcessorResponse.ExitRuleSet:
								return null;
							case RuleFlagProcessorResponse.LastRule:
								breakLoop = true;
								break;
						}

						// break the loop because we have reached the last rule as indicated by a flag
						if (breakLoop)
							break;
					}
					else if (containsChain)
					{
						skipNextChain = true;

						// reset the current URL back to the initial URL from the start of the chain
						currentContent = initialContent;
					}
					else if (previousContainsChain)
					{
						// reset the current URL back to the initial URL from the start of the chain
						currentContent = initialContent;
					}
				}

				//Manager.LogIf(LogLevel >= 9, "Output: " + currentContent, "OutRewrite");
				Manager.LogIf(LogLevel >= 1, "**********************************************************************************");
			}

			return currentContent;
		}

		#endregion
	}
}