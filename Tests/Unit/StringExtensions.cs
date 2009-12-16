using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManagedFusion.Rewriter.Test
{
	public static class StringExtensions
	{
		/// <summary>
		/// Converts a string to a byte array.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static byte[] ToByteArray(this string s)
		{
			return System.Text.Encoding.UTF8.GetBytes(s);
		}

		/// <summary>
		/// Converts a byte array to a string.
		/// </summary>
		/// <param name="ba"></param>
		/// <returns></returns>
		public static string GetString(this byte[] ba)
		{
			return System.Text.Encoding.UTF8.GetString(ba);
		}
	}
}
