
using System.Web;
using System;

namespace ManagedFusion.Rewriter.Rules.Flags
{
	public class ProxyFlag : IRuleFlag
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProxyFlag"/> class.
		/// </summary>
		public ProxyFlag()
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
			IHttpProxyHandler proxyHandler = null;

			if (Manager.Configuration.Rewriter.Proxy.UseAsyncProxy)
				proxyHandler = Activator.CreateInstance(Manager.ProxyType) as IHttpProxyHandler;
			else
				proxyHandler = Activator.CreateInstance(Manager.ProxyAsyncType) as IHttpProxyHandler;

			if (proxyHandler != null)
				proxyHandler.Init(context.SubstitutedUrl, context.RequestedUrl);

			context.HttpContext.Items.Add(Manager.ProxyHandlerStorageName, proxyHandler);
			Manager.LogIf(context.LogLevel >= 1, "Proxy: " + context.SubstitutedUrl, "Rewrite");
			
			return RuleFlagProcessorResponse.ExitRuleSet;
		}

		#endregion
	}
}
