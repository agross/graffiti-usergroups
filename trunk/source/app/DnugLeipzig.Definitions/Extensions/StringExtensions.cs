using System;

namespace DnugLeipzig.Definitions.Extensions
{
	public static class StringExtensions
	{
		public static bool IsNullOrEmptyTrimmed(this string value)
		{
			if (value == null)
			{
				return true;
			}

			return String.IsNullOrEmpty(value.Trim());
		}

		public static bool IsChecked(this string value)
		{
			if (value.IsNullOrEmptyTrimmed())
			{
				return false;
			}

			return String.Equals(value.Trim(), "on", StringComparison.OrdinalIgnoreCase);
		}

		public static DateTime AsEventDate(this string value)
		{
			DateTime date;
			if (DateTime.TryParse(value, out date))
			{
				return date;
			}

			return DateTime.MaxValue;
		}

		public static bool IsDate(this string value)
		{
			DateTime date;
			return DateTime.TryParse(value, out date);
		}

		public static int ToInt(this string value, int defaultValue)
		{
			int intValue;
			if (!int.TryParse(value, out intValue))
			{
				return defaultValue;
			}

			return intValue;
		}

		public static string StripDefaultAspx(this string url)
		{
			return url.Replace("default.aspx", "");
		}

		public static int LineCount(this string value)
		{
			if (value.IsNullOrEmptyTrimmed())
			{
				return 0;
			}

			return value.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).Length;
		}
	}
}