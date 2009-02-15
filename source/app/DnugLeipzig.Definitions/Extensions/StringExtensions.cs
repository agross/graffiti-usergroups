using System;

namespace DnugLeipzig.Definitions.Extensions
{
	public static class StringExtensions
	{
		public static bool IsNullOrEmpty(this string value)
		{
			if (value == null)
			{
				return true;
			}

			return String.IsNullOrEmpty(value.Trim());
		}

		public static bool HasValue(this string value)
		{
			return !value.IsNullOrEmpty();
		}

		public static bool IsSelected(this string value)
		{
			if (value.IsNullOrEmpty())
			{
				return false;
			}

			return String.Equals(value.Trim(), "on", StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsNotSelected(this string value)
		{
			return !IsSelected(value);
		}

		public static bool IsDate(this string value)
		{
			DateTime date;
			return DateTime.TryParse(value, out date);
		}

		public static DateTime ToDate(this string value)
		{
			return DateTime.Parse(value);
		}
		
		public static DateTime ToDate(this string value, DateTime defaultValue)
		{
			return value.IsDate() ? DateTime.Parse(value) : defaultValue;
		}

		public static DateTime AsEventDate(this string value)
		{
			return value.ToDate(DateTime.MaxValue);
		}

		public static bool IsInt(this string value)
		{
			int intValue;
			return int.TryParse(value, out intValue);
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

		public static bool IsInRange<T>(this T value, T minValue, T maxValue) where T : IComparable
		{
			return value.CompareTo(minValue) >= 0 && value.CompareTo(maxValue) <= 0;
		}

		public static string StripDefaultAspx(this string url)
		{
			return url.Replace("default.aspx", "");
		}
	}
}