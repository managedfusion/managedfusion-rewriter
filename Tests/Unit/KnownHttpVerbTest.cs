using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManagedFusion.Rewriter.Test
{
	/// <summary>
	/// Summary description for KnownHttpVerbTest
	/// </summary>
	[TestClass]
	public class KnownHttpVerbTest
	{
		[TestMethod]
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

		[TestMethod]
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

		[TestMethod]
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

		[TestMethod]
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

		[TestMethod]
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

		[TestMethod]
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

		[TestMethod]
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

		[TestMethod]
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
