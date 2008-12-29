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
			return value.IsNullOrEmptyTrimmed();
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

	public class When_the_line_count_of_a_string_is_computed : Spec
	{
		protected static int Because(string value)
		{
			return value.LineCount();
		}

		[RowTest]
		[Row("", 0)]
		[Row("    ", 0)]
		[Row(null, 0)]
		[Row("foo", 1)]
		[Row("foo\nbar", 2)]
		[Row("foo\r\nbar", 2)]
		[Row("foo\r\nbar\n", 2)]
		[Row("foo\r\nbar\r\n", 2)]
		[Row("foo\r\nbar\r\n\r\n\r\n\r\n\r\n", 2)]
		[Row("foo\r\nbar\r\n\r\nbaz\r\n\r\n\r\n", 3)]
		public void ShouldReturnCorrectLineCount(string value, int expectedLineCount)
		{
			Assert.AreEqual(expectedLineCount,
			                Because(value),
			                "Should have returned a line count of {0} for value '{1}'.",
			                expectedLineCount,
			                value);
		}
	}
}