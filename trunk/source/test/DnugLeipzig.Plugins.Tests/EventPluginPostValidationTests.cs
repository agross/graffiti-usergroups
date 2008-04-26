using System;

using Graffiti.Core;

using MbUnit.Framework;

namespace DnugLeipzig.Plugins.Tests
{
	[TestFixture]
	public class EventPluginPostValidationTests
	{
		const string StartDateField = "Start date field";
		const string EndDateField = "End date field";
		const string LocationUnknownField = "Location is unknown field";
		const string LocationField = "Location field";
		const string MaximumNumberOfRegistrationsField = "Maximum number of registrations field";
		const string NumberOfRegistrationsField = "Number of registrations field";
		const string RegistrationRecipientField = "Registration recipient field";
		const int EventCategoryId = 2;
		const string EventsCategoryName = "Events";
		EventPlugin _plugin;
		Post _post;

		[SetUp]
		public void SetUp()
		{
			_plugin = new EventPlugin();
			_plugin.CategoryName = EventsCategoryName;
			_plugin.StartDateField = StartDateField;
			_plugin.EndDateField = EndDateField;
			_plugin.LocationField = LocationField;
			_plugin.LocationUnknownField = LocationUnknownField;
			_plugin.MaximumNumberOfRegistrationsField = MaximumNumberOfRegistrationsField;
			_plugin.NumberOfRegistrationsField = NumberOfRegistrationsField;
			_plugin.RegistrationRecipientField = RegistrationRecipientField;

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
		[Row("invalid", ExpectedException = typeof(ValidationException))]
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
		[Row("invalid", ExpectedException = typeof(ValidationException))]
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

		[RowTest]
		[Row("")]
		[Row(null)]
		[Row("0")]
		[Row("100")]
		[Row("-100", ExpectedException = typeof(ValidationException))]
		[Row("invalid", ExpectedException = typeof(ValidationException))]
		public void MaximumNumberOfRegistrationsMustBePositiveInteger(string maximumNumberOfRegistrationsValue)
		{
			_post.CustomFields().Add(MaximumNumberOfRegistrationsField, maximumNumberOfRegistrationsValue);

			_plugin.Post_Validate(_post, EventArgs.Empty);
		}
		
		[RowTest]
		[Row("")]
		[Row(null)]
		[Row("0")]
		[Row("100")]
		[Row("-100", ExpectedException = typeof(ValidationException))]
		[Row("invalid", ExpectedException = typeof(ValidationException))]
		public void NumberOfRegistrationsMustBePositiveInteger(string numberOfRegistrationsValue)
		{
			_post.CustomFields().Add(NumberOfRegistrationsField, numberOfRegistrationsValue);

			_plugin.Post_Validate(_post, EventArgs.Empty);
		}

		[RowTest]
		[Row("")]
		[Row(null)]
		[Row("foo@example.com")]
		[Row("invalid", ExpectedException = typeof(ValidationException))]
		public void RegistrationRecipientMustBeValidIfGiven(string email)
		{
			_post.CustomFields().Add(RegistrationRecipientField, email);

			_plugin.Post_Validate(_post, EventArgs.Empty);
		}
	}
}