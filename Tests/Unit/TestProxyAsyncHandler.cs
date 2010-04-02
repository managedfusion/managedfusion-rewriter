using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;

namespace ManagedFusion.Rewriter.Test
{
	public class TestProxyAsyncHandler : ProxyAsyncHandler
	{
		protected override void OnRequestToTarget(HttpContext context, WebRequest request)
		{
		}

		protected override void OnResponseToClient(HttpContext context, WebResponse response)
		{
		}
	}
}
