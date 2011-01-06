using System;
using Moq;
using System.Web;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections;
using System.IO;

namespace ManagedFusion.Rewriter.Test
{
	public static class HttpHelpers
	{
		public static HttpContextBase MockHttpContext()
		{
			var context = new Mock<HttpContextBase>();
			var request = new Mock<HttpRequestBase>();
			var response = new Mock<HttpResponseBase>();
			var session = new Mock<HttpSessionStateBase>();
			var server = new Mock<HttpServerUtilityBase>();
			var headers = new Mock<NameValueCollection>();
			var items = new Mock<IDictionary>();

			response.SetupAllProperties();
			request.SetupGet(x => x.Headers).Returns(headers.Object);

			context.SetupGet(x => x.Items).Returns(items.Object);
			context.SetupGet(x => x.Request).Returns(request.Object);
			context.SetupGet(x => x.Response).Returns(response.Object);
			context.SetupGet(x => x.Session).Returns(session.Object);
			context.SetupGet(x => x.Server).Returns(server.Object);

			return context.Object;
		}

		public static HttpContextBase MockHttpContext(string url)
		{
			return MockHttpContext(new Uri(url));
		}

		public static HttpContextBase MockHttpContext(Uri url)
		{
			var context = MockHttpContext();
			context.Request.SetRequestUrl(url);
			return context;
		}

		private static NameValueCollection GetQueryStringParameters(Uri url)
		{
			var parameters = new NameValueCollection();
			var query = url.Query;

			if (String.IsNullOrEmpty(query))
				return parameters;

			string[] keys = query.Split("&".ToCharArray());

			foreach (string key in keys)
			{
				string[] part = key.Split("=".ToCharArray());
				parameters.Add(part[0], part[1]);
			}

			return parameters;
		}

		public static void SetHttpMethodResult(this HttpRequestBase request, string httpMethod)
		{
			Mock.Get(request)
				.Expect(req => req.HttpMethod)
				.Returns(httpMethod);
		}

		public static HttpRequestBase SetRequestUrl(this HttpRequestBase request, string url)
		{
			return SetRequestUrl(request, new Uri(url));
		}

		public static HttpRequestBase SetRequestUrl(this HttpRequestBase request, Uri url)
		{
			if (url == null)
				throw new ArgumentNullException("url");

			var mock = Mock.Get(request);

			mock.SetupGet(req => req.QueryString).Returns(GetQueryStringParameters(url));
			mock.SetupGet(req => req.Url).Returns(url);

			return request;
		}

		public static HttpRequestBase SetHeaders(this HttpRequestBase request, IDictionary<string, string> values)
		{
			if (values == null)
				values = new Dictionary<string, string>();

			var mock = Mock.Get(request);

			var headers = new NameValueCollection();
			foreach(var v in values)
				headers.Add(v.Key, v.Value);

			mock.SetupGet(req => req.Headers).Returns(headers);

			return request;
		}

		public static HttpRequestBase SetServerVariables(this HttpRequestBase request, IDictionary<string, string> values)
		{
			if (values == null)
				values = new Dictionary<string, string>();

			var mock = Mock.Get(request);

			var serverVars = new NameValueCollection();
			foreach (var v in values)
				serverVars.Add(v.Key, v.Value);

			mock.SetupGet(req => req.ServerVariables).Returns(serverVars);

			return request;
		}
	}
}
