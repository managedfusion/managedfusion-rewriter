using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ManagedFusion.Rewriter.Tests
{
	/// <summary>
	/// Summary description for KnownHttpVerbTest
	/// </summary>
	[TestFixture]
	public class KnownHttpVerbTest
	{
		[Test]
		public void OPTIONS_Verb()
		{
			// arrange
			var verb = "OPTIONS";

			// act
			var result = KnownHttpVerb.Parse(verb);

			// assert
			Assert.AreEqual(verb, result.Name);
			Assert.IsFalse(result.ConnectRequest);
			Assert.IsFalse(result.ContentBodyNotAllowed);
			Assert.IsFalse(result.ExpectNoContentResponse);
			Assert.IsFalse(result.RequireContentBody);
		}

		[Test]
		public void GET_Verb()
		{
			// arrange
			var verb = "GET";

			// act
			var result = KnownHttpVerb.Parse(verb);

			// assert
			Assert.AreEqual(verb, result.Name);
			Assert.IsFalse(result.ConnectRequest);
			Assert.IsTrue(result.ContentBodyNotAllowed);
			Assert.IsFalse(result.ExpectNoContentResponse);
			Assert.IsFalse(result.RequireContentBody);
		}

		[Test]
		public void HEAD_Verb()
		{
			// arrange
			var verb = "HEAD";

			// act
			var result = KnownHttpVerb.Parse(verb);

			// assert
			Assert.AreEqual(verb, result.Name);
			Assert.IsFalse(result.ConnectRequest);
			Assert.IsTrue(result.ContentBodyNotAllowed);
			Assert.IsTrue(result.ExpectNoContentResponse);
			Assert.IsFalse(result.RequireContentBody);
		}

		[Test]
		public void POST_Verb()
		{
			// arrange
			var verb = "POST";

			// act
			var result = KnownHttpVerb.Parse(verb);

			// assert
			Assert.AreEqual(verb, result.Name);
			Assert.IsFalse(result.ConnectRequest);
			Assert.IsFalse(result.ContentBodyNotAllowed);
			Assert.IsFalse(result.ExpectNoContentResponse);
			Assert.IsTrue(result.RequireContentBody);
		}

		[Test]
		public void PUT_Verb()
		{
			// arrange
			var verb = "PUT";

			// act
			var result = KnownHttpVerb.Parse(verb);

			// assert
			Assert.AreEqual(verb, result.Name);
			Assert.IsFalse(result.ConnectRequest);
			Assert.IsFalse(result.ContentBodyNotAllowed);
			Assert.IsFalse(result.ExpectNoContentResponse);
			Assert.IsTrue(result.RequireContentBody);
		}

		[Test]
		public void DELETE_Verb()
		{
			// arrange
			var verb = "DELETE";

			// act
			var result = KnownHttpVerb.Parse(verb);

			// assert
			Assert.AreEqual(verb, result.Name);
			Assert.IsFalse(result.ConnectRequest);
			Assert.IsFalse(result.ContentBodyNotAllowed);
			Assert.IsFalse(result.ExpectNoContentResponse);
			Assert.IsFalse(result.RequireContentBody);
		}

		[Test]
		public void TRACE_Verb()
		{
			// arrange
			var verb = "TRACE";

			// act
			var result = KnownHttpVerb.Parse(verb);

			// assert
			Assert.AreEqual(verb, result.Name);
			Assert.IsFalse(result.ConnectRequest);
			Assert.IsFalse(result.ContentBodyNotAllowed);
			Assert.IsFalse(result.ExpectNoContentResponse);
			Assert.IsFalse(result.RequireContentBody);
		}

		[Test]
		public void CONNECT_Verb()
		{
			// arrange
			var verb = "CONNECT";

			// act
			var result = KnownHttpVerb.Parse(verb);

			// assert
			Assert.AreEqual(verb, result.Name);
			Assert.IsTrue(result.ConnectRequest);
			Assert.IsTrue(result.ContentBodyNotAllowed);
			Assert.IsFalse(result.ExpectNoContentResponse);
			Assert.IsFalse(result.RequireContentBody);
		}
	}
}
