using System;

using DnugLeipzig.Definitions.Extensions;
using DnugLeipzig.Definitions.Specifications;
using DnugLeipzig.Runtime.Specifications;

namespace DnugLeipzig.Runtime.Validation
{
	internal static class ValidationExtensions
	{
		public static bool Exists(this string value)
		{
			return !value.IsNullOrEmptyTrimmed();
		}

		public static bool IsInRange<T>(this T value, T minValue, T maxValue) where T : IComparable
		{
			return value.CompareTo(minValue) >= 0 && value.CompareTo(maxValue) <= 0;
		}

		public static bool IsDate(this string value)
		{
			DateTime dateTimeValue;
			return DateTime.TryParse(value, out dateTimeValue);
		}

		public static bool IsInt(this string value)
		{
			int intValue;
			return int.TryParse(value, out intValue);
		}

		public static bool IsEmail(this string value)
		{
			EmailAddressSpecification spec = new EmailAddressSpecification();
			return spec.IsSatisfiedBy(value);
		}
	}
}