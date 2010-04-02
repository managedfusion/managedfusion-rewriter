using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManagedFusion.Rewriter.Test
{
	[TestClass]
	public class ProxyTest : BaseTest
	{
		[TestMethod]
		public void ProxyType()
		{
			Assert.AreEqual(Manager.ProxyType, typeof(TestProxyHandler));
		}

		[TestMethod]
		public void ProxyAsyncType()
		{
			Assert.AreEqual(Manager.ProxyAsyncType, typeof(TestProxyAsyncHandler));
		}
	}
}
