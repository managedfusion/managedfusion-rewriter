using System;
using System.Data;
using ManagedFusion.Rewriter;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace CoderJournal.Rewriter.Rules
{
	/// <summary>
	/// 
	/// </summary>
	public class PostQueryStringRuleAction : IRuleAction
	{
		#region IRuleAction Members

		/// <summary>
		/// Gets the pattern.
		/// </summary>
		/// <value>The pattern.</value>
		public Pattern Pattern
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the text.
		/// </summary>
		/// <value>The text.</value>
		public string Substitution
		{
			get;
			private set;
		}

		/// <summary>
		/// Processes the specified log level.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public void Execute(RuleContext context)
		{
			string inputUrl = context.CurrentUrl.GetComponents(UriComponents.PathAndQuery, UriFormat.UriEscaped);
			string sqlCommand = Pattern.Replace(inputUrl, Substitution, context);
			string substituedUrl;

			using (MySqlConnection connection = new MySqlConnection(Properties.Settings.Default.DatabaseConnection))
			using (MySqlCommand command = connection.CreateCommand())
			{
				command.CommandText = sqlCommand;
				command.CommandType = CommandType.Text;

				try
				{
					connection.Open();
					substituedUrl = command.ExecuteScalar() as string;
				}
				finally
				{
					connection.Close();
				}
			}

			context.SubstitutedUrl = new Uri(context.CurrentUrl, substituedUrl);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public virtual bool IsMatch(RuleContext context)
		{
			string testUrl = context.CurrentUrl.AbsolutePath;

			return Pattern.IsMatch(testUrl);
		}

		/// <summary>
		/// Inits the specified text.
		/// </summary>
		/// <param name="pattern">The pattern.</param>
		/// <param name="substitution">The text.</param>
		public void Init(Pattern pattern, string substitution)
		{
			Pattern = pattern;
			Substitution = substitution;
		}

		#endregion
	}
}
