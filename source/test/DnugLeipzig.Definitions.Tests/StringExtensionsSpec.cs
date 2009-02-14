using System;

using DnugLeipzig.Definitions.Extensions;
using DnugLeipzig.ForTesting;

using MbUnit.Framework;

namespace DnugLeipzig.Definitions.Tests
{
	public class When_a_string_is_tested_for_emptiness : Spec
	{
		protected static bool Because(string value)
		{
			return value.IsNullOrEmpty();
		}

		[RowTest]
		[Row("", true)]
		[Row("    ", true)]
		[Row(null, true)]
		[Row(" foo ", false)]
		public void It_should_indicate_if_the_string_is_empty(string value, bool expectedResult)
		{
			Assert.AreEqual(expectedResult,
			                Because(value),
			                "Should have returned {0} for value '{1}'.",
			                expectedResult,
			                value);
		}
	}

	public class When_an_invalid_date_value_is_converted_to_an_event_date : Spec
	{
		protected static DateTime Because(string value)
		{
			return value.AsEventDate();
		}

		[RowTest]
		[Row("")]
		[Row("    ")]
		[Row(null)]
		[Row(" foo ")]
		public void It_should_return_DateTime_MaxValue(string value)
		{
			Assert.AreEqual(DateTime.MaxValue,
			                Because(value),
			                "Should have returned DateTime.MaxValue because '{0}' is an invalid date.",
			                value);
		}
	}

	public class When_an_valid_date_value_is_converted_to_an_event_date : Spec
	{
		protected static DateTime Because(string value)
		{
			return value.AsEventDate();
		}

		[RowTest]
		[Row("11.12.2008", 2008, 12, 11)]
		[Row("11/12/2008", 2008, 12, 11)]
		public void It_should_return_DateTime_MaxValue(string value, int year, int month, int day)
		{
			Assert.AreEqual(new DateTime(year, month, day),
			                Because(value));
		}
	}
}