using System;
using System.Collections.Generic;
using System.Web;
using System.Collections.Specialized;
using System.Reflection;
using System.Collections;

namespace ManagedFusion.Rewriter.Test
{
	public static class HttpContextExtensions
	{
		/// <summary>
		/// Sets the read only name value collection.
		/// </summary>
		/// <param name="collection">The collection.</param>
		/// <param name="values">The values.</param>
		private static void SetReadOnlyNameValueCollection(NameValueCollection collection, IDictionary<string, string> values)
		{
			try
			{
				Type targetType = typeof(NameValueCollection);

				// get the property for setting readability
				PropertyInfo isReadOnlyProperty = targetType.GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);

				// set collection as read and write
				isReadOnlyProperty.SetValue(collection, false, null);

				// get the method to fill in the headers
				MethodInfo baseSetMethod = targetType.GetMethod("BaseSet", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(string), typeof(object) }, null);

				foreach (var param in values)
				{
					ArrayList list = new ArrayList();
					list.Add(param.Value);

					baseSetMethod.Invoke(collection, new object[] { param.Key, list });
				}

				// set collection as read only
				isReadOnlyProperty.SetValue(collection, true, null);
			}
			catch (MethodAccessException) { }
			catch (NullReferenceException) { }
		}

		/// <summary>
		/// Sets the server variables.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="values">The values.</param>
		/// <returns></returns>
		public static HttpContext SetServerVariables(this HttpContext context, IDictionary<string, string> values)
		{
			try
			{
				Type targetType = typeof(NameValueCollection);

				// get the property for setting readability
				PropertyInfo isReadOnlyProperty = targetType.GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);

				// set server variables as read and write
				isReadOnlyProperty.SetValue(context.Request.ServerVariables, false, null);

				targetType = typeof(HttpRequest);

				// get the method to fill in the headers
				MethodInfo addServerVariableToCollectionMethod = targetType.GetMethod("AddServerVariableToCollection", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(string), typeof(string) }, null);

				foreach (var param in values)
					addServerVariableToCollectionMethod.Invoke(context.Request, new object[] { param.Key, param.Value });

				// set server variables as read only
				isReadOnlyProperty.SetValue(context.Request.ServerVariables, true, null);

				// this calls the populate method
				var stringServerVariables = context.Request.ServerVariables.ToString();
			}
			catch (MethodAccessException) { }
			catch (NullReferenceException) { }

			return context;
		}

		/// <summary>
		/// Sets the header.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="values">The values.</param>
		/// <returns></returns>
		public static HttpContext SetHeader(this HttpContext context, IDictionary<string, string> values)
		{
			SetReadOnlyNameValueCollection(context.Request.Headers, values);
			return context;
		}
	}
}
