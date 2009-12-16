using System;
using System.Collections.Generic;
using System.Text;
using ManagedFusion.Rewriter.Conditions;
using System.Web;

namespace ManagedFusion.Rewriter
{
	public class ServerVariable
	{
		private static readonly ServerVariableType AllTypes = ServerVariableType.Cookies | ServerVariableType.Form | ServerVariableType.Headers | ServerVariableType.QueryString | ServerVariableType.ServerVariables;

		private string _name;
		private ServerVariableType _type;
		private object _key = new object();

		/// <summary>
		/// Initializes a new instance of the <see cref="RewriterServerVariable"/> class.
		/// </summary>
		protected internal ServerVariable()
		{
			_type = AllTypes;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RewriterServerVariable"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		public ServerVariable(string name)
		{
			_name = name;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RewriterServerVariable"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		public ServerVariable(string name, ServerVariableType type)
		{
			_name = name;
			_type = type;
		}

		/// <summary>
		/// Gets the text.
		/// </summary>
		/// <value>The text.</value>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Gets the type.
		/// </summary>
		/// <value>The type.</value>
		public ServerVariableType Type
		{
			get { return _type; }
			protected internal set { _type = value; }
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		private string Value
		{
			get { return HttpContext.Current.Items[_key] as string; }
			set { HttpContext.Current.Items[_key] = value; }
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public string GetValue(string input, RuleSetContext context)
		{
			// if the value has already been cached then return that value instead of reprocessing
			if (Value != null)
				return Value;

			string value = String.Empty;

			if ((_type & ServerVariableType.ServerVariables) != 0)
			{
				value = context.HttpContext.Request.ServerVariables[_name];

				// check to see if the value was found if not try the switch statement
				if (String.IsNullOrEmpty(value))
				{
					switch (_name)
					{
						case "TIME_YEAR":
							value = DateTime.Today.Year.ToString();
							break;
						case "TIME_MON":
							value = DateTime.Today.Month.ToString();
							break;
						case "TIME_DAY":
							value = DateTime.Today.Day.ToString();
							break;
						case "TIME_HOUR":
							value = DateTime.Now.Hour.ToString();
							break;
						case "TIME_MIN":
							value = DateTime.Now.Minute.ToString();
							break;
						case "TIME_SEC":
							value = DateTime.Now.Second.ToString();
							break;
						case "TIME_WDAY":
							value = DateTime.Today.DayOfWeek.ToString();
							break;
						case "TIME":
							value = DateTime.Now.ToString("f");
							break;
						case "API_VERSION":
							value = String.Empty;
							break;// TODO: figure out how to attack this
						case "THE_REQUEST":
							value = context.HttpContext.Request.ServerVariables["REQUEST_METHOD"] + " " + context.HttpContext.Request.ServerVariables["PATH_INFO"] + " " + context.HttpContext.Request.ServerVariables["SERVER_PROTOCOL"];
							break;
						case "REQUEST_URI":
							value = context.HttpContext.Request.ServerVariables["URL"];
							break;
						case "REQUEST_FILENAME":
							value = context.HttpContext.Request.ServerVariables["PATH_TRANSLATED"];
							break;
						case "IS_SUBREQ":
							value = String.IsNullOrEmpty(context.HttpContext.Request.Headers["X-Rewriter-Transfer"]) ? Boolean.FalseString : Boolean.TrueString;
							break;
					}
				}
			}
			else if ((_type & ServerVariableType.Headers) != 0)
				value = context.HttpContext.Request.Headers[_name];
			else if ((_type & ServerVariableType.QueryString) != 0)
				value = context.HttpContext.Request.QueryString[_name];
			else if ((_type & ServerVariableType.Form) != 0)
				value = context.HttpContext.Request.Form[_name];
			else if ((_type & ServerVariableType.Cookies) != 0)
			{
				HttpCookie cookie = context.HttpContext.Request.Cookies[_name];

				// if cookie was found set the value to the value of the cookie
				if (cookie != null)
					value = cookie.Value;
			}

			Manager.LogIf(context.LogLevel >= 2, "Input: " + value, context.LogCategory);

			Value = value;
			return value;
		}
	}
}
