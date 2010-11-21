using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ManagedFusion.Rewriter.Rules.Flags;

namespace ManagedFusion.Rewriter.Rules.Test
{
	[TestFixture]
	public class RuleFlagProcessorTest
	{
		[Test]
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
			Assert.IsInstanceOf<NoCaseFlag>(list[0]);
			Assert.IsInstanceOf<NoEscapeFlag>(list[1]);
			Assert.IsInstanceOf<ResponseCookieFlag>(list[2]);
			Assert.IsInstanceOf<LastFlag>(list[3]);
		}

		[Test]
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
			Assert.IsInstanceOf<NoCaseFlag>(list[0]);
			Assert.IsInstanceOf<NoEscapeFlag>(list[1]);
			Assert.IsInstanceOf<ResponseCookieFlag>(list[2]);
			Assert.IsInstanceOf<ProxyFlag>(list[3]);
		}

		[Test]
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
			Assert.IsInstanceOf<NoCaseFlag>(list[0]);
			Assert.IsInstanceOf<NoEscapeFlag>(list[1]);
			Assert.IsInstanceOf<ResponseCookieFlag>(list[2]);
			Assert.IsInstanceOf<RedirectFlag>(list[3]);
		}

		[Test]
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
			Assert.IsInstanceOf<NoCaseFlag>(list[0]);
			Assert.IsInstanceOf<NoEscapeFlag>(list[1]);
			Assert.IsInstanceOf<ResponseCookieFlag>(list[2]);
			Assert.IsInstanceOf<RedirectFlag>(list[3]);
			Assert.IsInstanceOf<LastFlag>(list[4]);
		}

		[Test]
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
			Assert.IsInstanceOf<NoCaseFlag>(list[0]);
			Assert.IsInstanceOf<NoEscapeFlag>(list[1]);
			Assert.IsInstanceOf<ResponseCookieFlag>(list[2]);
			Assert.IsInstanceOf<LastFlag>(list[3]);
		}

		[Test]
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
			Assert.IsInstanceOf<NoCaseFlag>(list[0]);
			Assert.IsInstanceOf<NoEscapeFlag>(list[1]);
			Assert.IsInstanceOf<ResponseCookieFlag>(list[2]);
			Assert.IsInstanceOf<LastFlag>(list[3]);
		}

		[Test]
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
			Assert.IsInstanceOf<NoCaseFlag>(list[0]);
			Assert.IsInstanceOf<NoEscapeFlag>(list[1]);
			Assert.IsInstanceOf<ResponseCookieFlag>(list[2]);
			Assert.IsInstanceOf<LastFlag>(list[3]);
			Assert.IsInstanceOf<LastFlag>(list[4]);
		}

		[Test]
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
			Assert.IsInstanceOf<NoCaseFlag>(list[0]);
			Assert.IsInstanceOf<NoEscapeFlag>(list[1]);
			Assert.IsInstanceOf<ResponseCookieFlag>(list[2]);
			Assert.IsInstanceOf<ProxyFlag>(list[3]);
			Assert.IsInstanceOf<LastFlag>(list[4]);
			Assert.IsInstanceOf<LastFlag>(list[5]);
		}

		[Test]
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
			Assert.IsInstanceOf<NoCaseFlag>(list[0]);
			Assert.IsInstanceOf<NoEscapeFlag>(list[1]);
			Assert.IsInstanceOf<ResponseCookieFlag>(list[2]);
			Assert.IsInstanceOf<ProxyFlag>(list[3]);
			Assert.IsInstanceOf<LastFlag>(list[4]);
			Assert.IsInstanceOf<LastFlag>(list[5]);
		}
	}
}
