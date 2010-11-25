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
using System.Web.Caching;
using ManagedFusion.Rewriter.Configuration;
using System.Web.Hosting;

namespace ManagedFusion.Rewriter.Engines
{
	/// <summary>
	/// 
	/// </summary>
	public class ApacheEngine : IRewriterEngine
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ApacheEngine"/> class.
		/// </summary>
		public ApacheEngine()
		{
			Paths = new Dictionary<string, ApacheRuleSet>();

			// set file name
			FileName = Manager.Configuration.Rules.Apache.DefaultFileName;

			// set physical application path
			PhysicalApplicationPath = Manager.Configuration.Rules.Apache.DefaultPhysicalApplicationPath;

			// if there is no physical application path set then get it from the request
			if (String.IsNullOrEmpty(PhysicalApplicationPath))
			{
				try
				{
					PhysicalApplicationPath = HostingEnvironment.ApplicationPhysicalPath;
				}
				finally
				{
					if (String.IsNullOrEmpty(PhysicalApplicationPath))
						PhysicalApplicationPath = Environment.CurrentDirectory;
				}
			}
		}

		protected IDictionary<string, ApacheRuleSet> Paths { get; private set; }

		/// <summary>
		/// Gets or sets the name of the file.
		/// </summary>
		/// <value>The name of the file.</value>
		private string FileName { get; set; }

		/// <summary>
		/// Gets or sets the physical application path.
		/// </summary>
		/// <value>The physical application path.</value>
		private string PhysicalApplicationPath { get; set; }

		/// <summary>
		/// Gets the <see cref="ManagedFusion.Rewriter.RuleSet"/> with the specified relative path.
		/// </summary>
		/// <param name="relativePath">The relative path.</param>
		/// <returns></returns>
		/// <value></value>
		private ApacheRuleSet GetRuleSet(string relativePath)
		{
			if (String.IsNullOrEmpty(relativePath))
				throw new ArgumentNullException("relativePath");

			bool found;

			// normalize the path before processing
			relativePath = relativePath.Replace('\\', '/');

			do
			{
				// make sure the relative path is at least the root
				if (String.IsNullOrEmpty(relativePath))
					relativePath = "/";

				found = Paths.ContainsKey(relativePath);

				// if not found then go to the parent directory of the current relative path
				if (!found)
					relativePath = relativePath.Substring(0, relativePath.LastIndexOf('/'));

			} while (!found && relativePath != "/");

			if (!found)
				return null;

			return Paths[relativePath];
		}

		#region Scan Directory For Rules

		/// <summary>
		/// Scans the directories for rules.
		/// </summary>
		/// <param name="refreshDir">The refresh dir.</param>
		private void ScanDirectoriesForRules(DirectoryInfo refreshDir)
		{
			if (refreshDir == null)
				refreshDir = new DirectoryInfo(PhysicalApplicationPath);

			FileInfo file = new FileInfo(refreshDir.FullName + FileName);
			if (file.Exists)
				Add(GetRelativePath(file), file);

			try
			{
				// scan all sub directories
				foreach (DirectoryInfo dir in refreshDir.GetDirectories())
					ScanDirectoriesForRules(dir);
			}
			catch (UnauthorizedAccessException) { }
		}

		/// <summary>
		/// Gets the relative path.
		/// </summary>
		/// <param name="file">The file.</param>
		/// <returns></returns>
		private string GetRelativePath(FileInfo file)
		{
			if (file == null)
				throw new ArgumentNullException("file");

			if (!file.Exists)
				throw new ArgumentException("The file needs to exist.", "file");

			Uri fromFile = new Uri(PhysicalApplicationPath);
			Uri toFile = new Uri(file.DirectoryName + Path.DirectorySeparatorChar);
			Uri relativeFile = fromFile.MakeRelativeUri(toFile);

			// normalize the path characters to the web format
			string path = relativeFile.ToString().Replace('\\', '/');

			// if no path is found then just use the root
			if (String.IsNullOrEmpty(path))
				path = "/";

			return path;
		}

		#endregion

		#region Use Defined Rules

		/// <summary>
		/// Uses the defined rules.
		/// </summary>
		private void UseDefinedRules()
		{
			for (int i = 0; i < Manager.Configuration.Rules.Apache.Count; i++)
			{
				ApacheRuleSetItem item = Manager.Configuration.Rules.Apache[i];
				FileInfo file = new FileInfo(item.ConfigPath);
		
				if (!item.ApplicationPath.StartsWith("/"))
					throw new RewriterEngineException("Your ApplicationPath, " + item.ApplicationPath + ", in the config file must start with \"/\".");

				if (!file.Exists)
					throw new RewriterEngineException("Your ConfigPath, " + item.ConfigPath + ", in the config file does not exist.");

				// add the rewriter to the available rewriters
				Add(item.ApplicationPath, file);
			}
		}

		#endregion

		#region RuleSet Expiration

		/// <summary>
		/// Rules the set expired.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <param name="reason">The reason.</param>
		private void RuleSetExpired(string key, object value, CacheItemRemovedReason reason)
		{
			var ruleSet = GetRuleSet(key);

			if (ruleSet != null)
				ruleSet.RefreshRules();

			// start remonitor the rule set
			AddRuleSetMonitoring(key, value as string);
		}

		/// <summary>
		/// Adds the rule set monitoring.
		/// </summary>
		/// <param name="relativePath">The relative path.</param>
		/// <param name="fullPath">The full path.</param>
		protected void AddRuleSetMonitoring(string relativePath, string fullPath)
		{
			// add a cache place holder to monitor the file system for changes
			HttpRuntime.Cache.Insert(
				relativePath,
				fullPath,
				new CacheDependency(fullPath),
				Cache.NoAbsoluteExpiration,
				Cache.NoSlidingExpiration,
				CacheItemPriority.NotRemovable,
				RuleSetExpired
			);
		}

		#endregion

		/// <summary>
		/// Adds the specified relative path.
		/// </summary>
		/// <param name="relativePath">The relative path.</param>
		/// <param name="file">The file.</param>
		protected virtual void Add(string relativePath, FileInfo file)
		{
			ApacheRuleSet rule = new ApacheRuleSet(relativePath, file);
			Paths.Add(relativePath, rule);

			// start monitoring the rule set
			AddRuleSetMonitoring(relativePath, file.FullName);
		}

		#region IRewriterEngine Members

		/// <summary>
		/// Scans the directories for rules.
		/// </summary>
		public virtual void Init()
		{
			Paths.Clear();

			// use defined rules if they are set
			// else scan the directories for values
			if (Manager.Configuration.Rules.Apache.Count > 0)
				UseDefinedRules();
			else
				ScanDirectoriesForRules(null);

			// build all rule sets
			RefreshRules();
		}

		/// <summary>
		/// Refreshes the rules.
		/// </summary>
		public virtual void RefreshRules()
		{
			foreach (ApacheRuleSet set in Paths.Values)
				set.RefreshRules();
		}

		/// <summary>
		/// Runs the rules.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public virtual Uri RunRules(HttpContext context)
		{
			var url = new Uri(context.Request.Url, context.Request.RawUrl);
			string path = context.Request.ApplicationPath;

			// get the ruleset to execute
			ApacheRuleSet ruleSet = GetRuleSet(path);

			// if no rule set was found there is nothing more to do
			if (ruleSet == null)
				return null;

			Uri rewritenUrl = ruleSet.RunRules(context, url);

			return rewritenUrl;
		}

		/// <summary>
		/// Runs the output rules.
		/// </summary>
		/// <param name="context">The context.</param>
		public virtual void RunOutputRules(HttpContext context)
		{
			string path = context.Request.ApplicationPath;

			// get the ruleset to execute
			ApacheRuleSet ruleSet = GetRuleSet(path);

			// if no rule set was found there is nothing more to do
			if (ruleSet == null)
				return;

			if (ruleSet.OutputRuleCount > 0)
				context.Response.Filter = new RuleSetOutputFilter(context.Response.Filter, context, ruleSet);
		}

		#endregion
	}
}
