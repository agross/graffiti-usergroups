using System;

namespace DnugLeipzig.Extensions.Extensions
{
	public static class StringExtensions
	{
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

		public static int ToIntMin(this string value)
		{
			int intValue;
			if (!int.TryParse(value, out intValue))
			{
				return int.MinValue;
			}

			return intValue;
		}

		public static int ToIntMax(this string value)
		{
			int intValue;
			if (!int.TryParse(value, out intValue))
			{
				return int.MaxValue;
			}

			return intValue;
		}

		public static string StripDefaultAspx(this string url)
		{
			return url.Replace("default.aspx", "");
		}
	}
}