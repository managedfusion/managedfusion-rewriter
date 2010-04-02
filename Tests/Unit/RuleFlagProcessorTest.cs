using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedFusion.Rewriter.Rules.Flags;

namespace ManagedFusion.Rewriter.Rules.Test
{
	[TestClass]
	public class RuleFlagProcessorTest
	{
		[TestMethod]
		public void OrderOfOpperations_Followed()
		{
			// arrange
			var processor = new RuleFlagProcessor();

			// act
			processor.BeginAdd();
			processor.Add(new NoCaseFlag());
			processor.Add(new NoEscapeFlag());
			processor.Add(new ResponseCookieFlag("test", "test", "test", null, "/"));
			processor.Add(new LastFlag());
			processor.EndAdd();

			// assert
			var list = processor.ToList();
			Assert.IsInstanceOfType(list[0], typeof(NoCaseFlag));
			Assert.IsInstanceOfType(list[1], typeof(NoEscapeFlag));
			Assert.IsInstanceOfType(list[2], typeof(ResponseCookieFlag));
			Assert.IsInstanceOfType(list[3], typeof(LastFlag));
		}

		[TestMethod]
		public void OrderOfOpperations_Followed_2()
		{
			// arrange
			var processor = new RuleFlagProcessor();

			// act
			processor.BeginAdd();
			processor.Add(new NoCaseFlag());
			processor.Add(new NoEscapeFlag());
			processor.Add(new ResponseCookieFlag("test", "test", "test", null, "/"));
			processor.Add(new ProxyFlag());
			processor.EndAdd();

			// assert
			var list = processor.ToList();
			Assert.IsInstanceOfType(list[0], typeof(NoCaseFlag));
			Assert.IsInstanceOfType(list[1], typeof(NoEscapeFlag));
			Assert.IsInstanceOfType(list[2], typeof(ResponseCookieFlag));
			Assert.IsInstanceOfType(list[3], typeof(ProxyFlag));
		}

		[TestMethod]
		public void OrderOfOpperations_Followed_3()
		{
			// arrange
			var processor = new RuleFlagProcessor();

			// act
			processor.BeginAdd();
			processor.Add(new NoCaseFlag());
			processor.Add(new NoEscapeFlag());
			processor.Add(new ResponseCookieFlag("test", "test", "test", null, "/"));
			processor.Add(new RedirectFlag(302));
			processor.EndAdd();

			// assert
			var list = processor.ToList();
			Assert.IsInstanceOfType(list[0], typeof(NoCaseFlag));
			Assert.IsInstanceOfType(list[1], typeof(NoEscapeFlag));
			Assert.IsInstanceOfType(list[2], typeof(ResponseCookieFlag));
			Assert.IsInstanceOfType(list[3], typeof(RedirectFlag));
		}

		[TestMethod]
		public void OrderOfOpperations_Followed_4()
		{
			// arrange
			var processor = new RuleFlagProcessor();

			// act
			processor.BeginAdd();
			processor.Add(new NoCaseFlag());
			processor.Add(new NoEscapeFlag());
			processor.Add(new ResponseCookieFlag("test", "test", "test", null, "/"));
			processor.Add(new RedirectFlag(302));
			processor.Add(new LastFlag());
			processor.EndAdd();

			// assert
			var list = processor.ToList();
			Assert.IsInstanceOfType(list[0], typeof(NoCaseFlag));
			Assert.IsInstanceOfType(list[1], typeof(NoEscapeFlag));
			Assert.IsInstanceOfType(list[2], typeof(ResponseCookieFlag));
			Assert.IsInstanceOfType(list[3], typeof(RedirectFlag));
			Assert.IsInstanceOfType(list[4], typeof(LastFlag));
		}

		[TestMethod]
		public void OrderOfOpperations_NotFollowed_First()
		{
			// arrange
			var processor = new RuleFlagProcessor();

			// act
			processor.BeginAdd();
			processor.Add(new LastFlag());
			processor.Add(new NoCaseFlag());
			processor.Add(new NoEscapeFlag());
			processor.Add(new ResponseCookieFlag("test", "test", "test", null, "/"));
			processor.EndAdd();

			// assert
			var list = processor.ToList();
			Assert.IsInstanceOfType(list[0], typeof(NoCaseFlag));
			Assert.IsInstanceOfType(list[1], typeof(NoEscapeFlag));
			Assert.IsInstanceOfType(list[2], typeof(ResponseCookieFlag));
			Assert.IsInstanceOfType(list[3], typeof(LastFlag));
		}

		[TestMethod]
		public void OrderOfOpperations_NotFollowed_Middle()
		{
			// arrange
			var processor = new RuleFlagProcessor();

			// act
			processor.BeginAdd();
			processor.Add(new NoCaseFlag());
			processor.Add(new NoEscapeFlag());
			processor.Add(new LastFlag());
			processor.Add(new ResponseCookieFlag("test", "test", "test", null, "/"));
			processor.EndAdd();

			// assert
			var list = processor.ToList();
			Assert.IsInstanceOfType(list[0], typeof(NoCaseFlag));
			Assert.IsInstanceOfType(list[1], typeof(NoEscapeFlag));
			Assert.IsInstanceOfType(list[2], typeof(ResponseCookieFlag));
			Assert.IsInstanceOfType(list[3], typeof(LastFlag));
		}

		[TestMethod]
		public void OrderOfOpperations_NotFollowed_LastMiddle()
		{
			// arrange
			var processor = new RuleFlagProcessor();

			// act
			processor.BeginAdd();
			processor.Add(new NoCaseFlag());
			processor.Add(new LastFlag());
			processor.Add(new NoEscapeFlag());
			processor.Add(new ResponseCookieFlag("test", "test", "test", null, "/"));
			processor.Add(new LastFlag());
			processor.EndAdd();

			// assert
			var list = processor.ToList();
			Assert.IsInstanceOfType(list[0], typeof(NoCaseFlag));
			Assert.IsInstanceOfType(list[1], typeof(NoEscapeFlag));
			Assert.IsInstanceOfType(list[2], typeof(ResponseCookieFlag));
			Assert.IsInstanceOfType(list[3], typeof(LastFlag));
			Assert.IsInstanceOfType(list[4], typeof(LastFlag));
		}

		[TestMethod]
		public void OrderOfOpperations_NotFollowed_LastMiddleFirst()
		{
			// arrange
			var processor = new RuleFlagProcessor();

			// act
			processor.BeginAdd();
			processor.Add(new ProxyFlag());
			processor.Add(new NoCaseFlag());
			processor.Add(new LastFlag());
			processor.Add(new NoEscapeFlag());
			processor.Add(new ResponseCookieFlag("test", "test", "test", null, "/"));
			processor.Add(new LastFlag());
			processor.EndAdd();

			// assert
			var list = processor.ToList();
			Assert.IsInstanceOfType(list[0], typeof(NoCaseFlag));
			Assert.IsInstanceOfType(list[1], typeof(NoEscapeFlag));
			Assert.IsInstanceOfType(list[2], typeof(ResponseCookieFlag));
			Assert.IsInstanceOfType(list[3], typeof(ProxyFlag));
			Assert.IsInstanceOfType(list[4], typeof(LastFlag));
			Assert.IsInstanceOfType(list[5], typeof(LastFlag));
		}

		[TestMethod]
		public void NotBulkLoaded()
		{
			// arrange
			var processor = new RuleFlagProcessor();

			// act

			processor.Add(new ProxyFlag());
			processor.Add(new NoCaseFlag());
			processor.Add(new LastFlag());
			processor.Add(new NoEscapeFlag());
			processor.Add(new ResponseCookieFlag("test", "test", "test", null, "/"));
			processor.Add(new LastFlag());

			// assert
			var list = processor.ToList();
			Assert.IsInstanceOfType(list[0], typeof(NoCaseFlag));
			Assert.IsInstanceOfType(list[1], typeof(NoEscapeFlag));
			Assert.IsInstanceOfType(list[2], typeof(ResponseCookieFlag));
			Assert.IsInstanceOfType(list[3], typeof(ProxyFlag));
			Assert.IsInstanceOfType(list[4], typeof(LastFlag));
			Assert.IsInstanceOfType(list[5], typeof(LastFlag));
		}
	}
}
