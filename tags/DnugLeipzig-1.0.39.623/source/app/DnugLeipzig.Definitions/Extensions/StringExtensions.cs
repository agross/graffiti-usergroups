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
	}
}