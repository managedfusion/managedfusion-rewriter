using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace ManagedFusion.Rewriter.Test
{
	[TestFixture]
	public class ProxyTest : BaseTest
	{
		[Test]
		public void ProxyType()
		{
			Assert.AreEqual(Manager.ProxyType, typeof(TestProxyHandler));
		}

		[Test]
		public void ProxyAsyncType()
		{
			Assert.AreEqual(Manager.ProxyAsyncType, typeof(TestProxyAsyncHandler));
		}
	}
}
