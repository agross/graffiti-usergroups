using System;

using DnugLeipzig.Definitions.Extensions;
using DnugLeipzig.ForTesting;

using MbUnit.Framework;

namespace DnugLeipzig.Definitions.Tests
{
	public class StringExtensionsTests : Spec
	{
		[RowTest]
		[Row("", true)]
		[Row("    ", true)]
		[Row(null, true)]
		[Row(" foo ", false)]
		public void IsNullOrEmptyTrimmedRow(string value, bool expectedResult)
		{
			Assert.AreEqual(expectedResult,
			                value.IsNullOrEmptyTrimmed(),
			                "Should have returned {0} for value '{1}'.",
			                expectedResult,
			                value);
		}

		[RowTest]
		[Row("")]
		[Row("    ")]
		[Row(null)]
		[Row(" foo ")]
		public void ShouldReturnDateTimeMaxValueWhenParsingInvalidEventDate(string value)
		{
			Assert.AreEqual(DateTime.MaxValue,
			                value.AsEventDate(),
			                "Should have returned DateTime.MaxValue because '{0}' is an invalid date.",
			                value);
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
			                value.LineCount(),
			                "Should have returned a line count of {0} for value '{1}'.",
			                expectedLineCount,
			                value);
		}
	}
}