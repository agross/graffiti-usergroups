using System;

using Graffiti.Core;

using MbUnit.Framework;

namespace DnugLeipzig.Plugins.Tests
{
	[TestFixture]
	public class EventPlugValidtionTests
	{
		const string StartDateField = "Start date field";
		const string EndDateField = "End date field";
		const string LocationUnknownField = "Location is unknown field";
		const string LocationField = "Location field";
		const int EventCategoryId = 2;
		EventPlugin _plugin;
		Post _post;

		[SetUp]
		public void SetUp()
		{
			_plugin = new EventPlugin();
			_plugin.CategoryName = "Events";
			_plugin.StartDateField = StartDateField;
			_plugin.EndDateField = EndDateField;
			_plugin.LocationField = LocationField;
			_plugin.LocationUnknownField = LocationUnknownField;

			_post = new Post();
			_post.CategoryId = EventCategoryId;
		}

		[RowTest]
		[Row("")]
		[Row("     ")]
		[Row(null)]
		[Row("2008/2/3")]
		[Row("2008/2/3 8:00 AM")]
		[Row("3.2.2008 8:00")]
		[Row("invalid value", ExpectedException = typeof(ValidationException))]
		public void ShouldValidateStartDateIfSet(string startDateValue)
		{
			_post.CustomFields().Add(StartDateField, startDateValue);

			_plugin.Post_Validate(_post, EventArgs.Empty);	
		}
		
		[RowTest]
		[Row("")]
		[Row("     ")]
		[Row(null)]
		[Row("2008/2/3")]
		[Row("2008/2/3 8:00 AM")]
		[Row("3.2.2008 8:00")]
		[Row("invalid value", ExpectedException = typeof(ValidationException))]
		public void ShouldValidateEndDateIfSet(string endDateValue)
		{
			_post.CustomFields().Add(StartDateField, "2008/2/3");
			_post.CustomFields().Add(EndDateField, endDateValue);

			_plugin.Post_Validate(_post, EventArgs.Empty);	
		}

		[Test]
		[ExpectedException(typeof(ValidationException))]
		public void RequiresStartDateIfEndDateIsSet()
		{
			_post.CustomFields().Add(EndDateField, "2008/2/3");
			_plugin.Post_Validate(_post, EventArgs.Empty);	
		}

		[RowTest]
		[Row("", "")]
		[Row("   ", "    ")]
		[Row(null, null)]
		[Row("2008/2/3", "2008/2/3")]
		[Row("2008/2/3", "2008/2/3 8:00 AM")]
		[Row("2008/2/3 7:00 AM", "2008/2/3 8:00 AM")]
		[Row("2008/2/3 7:00 AM", "2008/2/3", ExpectedException = typeof(ValidationException))]
		[Row("2008/2/3 7:00 AM", "2008/2/3 6:00 AM", ExpectedException = typeof(ValidationException))]
		public void EndDateShouldBeGreaterEqualThanStartDate(string startDateValue, string endDateValue)
		{
			_post.CustomFields().Add(StartDateField, startDateValue);
			_post.CustomFields().Add(EndDateField, endDateValue);
			_plugin.Post_Validate(_post, EventArgs.Empty);
		}

		[Test]
		[ExpectedException(typeof(ValidationException))]
		public void LocationUnknownMustBeUnCheckedIfLocationFieldIsNotEmpty()
		{
			_post.CustomFields().Add(LocationField, "some location");
			_post.CustomFields().Add(LocationUnknownField, "on");

			_plugin.Post_Validate(_post, EventArgs.Empty);
		}
	}
}