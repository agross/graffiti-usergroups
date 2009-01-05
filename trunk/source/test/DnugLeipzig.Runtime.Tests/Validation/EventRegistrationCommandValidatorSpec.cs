using System.Linq;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Definitions.Validation;
using DnugLeipzig.ForTesting;
using DnugLeipzig.Runtime.Validation;

using MbUnit.Framework;

namespace DnugLeipzig.Runtime.Tests.Validation
{
	public class When_a_valid_event_registration_request_is_validated : With_event_registration_validator
	{
		protected override IEventRegistrationCommand CreateDataToValidate()
		{
			return Create.New.EventRegistration()
				.Register(new[] { 10, 42 })
				.ForAttendee("First name last name")
				.WithEmail("foo@bar.com")
				.SendConfirmationToAttendee()
				.WithFormOfAddress("Mr")
				.WithOccupation("IT Pro")
				.Build();
		}

		[Test]
		public void It_should_not_return_any_notifications()
		{
			Assert.AreEqual(0, Notifications.Count());
		}
	}

	public class When_an_event_registration_request_is_validated_and_the_name_is_missing
		: With_event_registration_validator
	{
		protected override IEventRegistrationCommand CreateDataToValidate()
		{
			return Create.New.EventRegistration()
				.Register(new[] { 10, 42 })
				.WithEmail("foo@bar.com")
				.SendConfirmationToAttendee()
				.WithFormOfAddress("Mr")
				.WithOccupation("IT Pro")
				.Build();
		}

		[Test]
		public void It_should_return_one_error()
		{
			Assert.AreEqual(1, Notifications.Count());
		}

		[Test]
		public void It_should_indicate_that_the_name_is_missing()
		{
			Assert.AreEqual("Please enter your name.", Notifications.First().Message);
		}
	}

	public class When_an_event_registration_request_is_validated_and_there_are_no_events_to_register
		: With_event_registration_validator
	{
		protected override IEventRegistrationCommand CreateDataToValidate()
		{
			return Create.New.EventRegistration()
				.ForAttendee("First name last name")
				.WithEmail("foo@bar.com")
				.SendConfirmationToAttendee()
				.WithFormOfAddress("Mr")
				.WithOccupation("IT Pro")
				.Build();
		}

		[Test]
		public void It_should_return_one_error()
		{
			Assert.AreEqual(1, Notifications.Count());
		}

		[Test]
		public void It_should_indicate_that_there_are_no_events_to_register()
		{
			Assert.AreEqual("Please select at least one event to register for.", Notifications.First().Message);
		}
	}
	
	public class When_an_event_registration_request_is_validated_and_the_email_is_missing
		: With_event_registration_validator
	{
		protected override IEventRegistrationCommand CreateDataToValidate()
		{
			return Create.New.EventRegistration()
				.Register(new[] { 10, 42 })
				.ForAttendee("First name last name")
				.SendConfirmationToAttendee()
				.WithFormOfAddress("Mr")
				.WithOccupation("IT Pro")
				.Build();
		}

		[Test]
		public void It_should_return_one_error()
		{
			Assert.AreEqual(1, Notifications.Count());
		}

		[Test]
		public void It_should_indicate_that_the_email_is_missing()
		{
			// TODO
			//Assert.AreEqual("Please enter your e-mail address.", Notifications.First().Message);
		}
	}
	
	public class When_an_event_registration_request_is_validated_and_the_email_is_invalid
		: With_event_registration_validator
	{
		protected override IEventRegistrationCommand CreateDataToValidate()
		{
			return Create.New.EventRegistration()
				.Register(new[] { 10, 42 })
				.ForAttendee("First name last name")
				.WithEmail("invalid e-mail")
				.SendConfirmationToAttendee()
				.WithFormOfAddress("Mr")
				.WithOccupation("IT Pro")
				.Build();
		}

		[Test]
		public void It_should_return_one_error()
		{
			Assert.AreEqual(1, Notifications.Count());
		}

		[Test]
		public void It_should_indicate_that_the_email_is_invalid()
		{
			// TODO
			//Assert.AreEqual("The e-mail address 'invalid e-mail' is invalid.", Notifications.First().Message);
		}
	}
	
	public class When_an_event_registration_request_is_validated_and_form_of_address_in_missing
		: With_event_registration_validator
	{
		protected override IEventRegistrationCommand CreateDataToValidate()
		{
			return Create.New.EventRegistration()
				.Register(new[] { 10, 42 })
				.ForAttendee("First name last name")
				.WithEmail("foo@bar.com")
				.SendConfirmationToAttendee()
				.WithOccupation("IT Pro")
				.Build();
		}

		[Test]
		public void It_should_return_one_error()
		{
			Assert.AreEqual(1, Notifications.Count());
		}

		[Test]
		public void It_should_indicate_that_the_form_of_address_is_missing()
		{
			Assert.AreEqual("Please select a form of address.", Notifications.First().Message);
		}
	}
	
	public class When_an_event_registration_request_is_validated_and_occupation_in_missing
		: With_event_registration_validator
	{
		protected override IEventRegistrationCommand CreateDataToValidate()
		{
			return Create.New.EventRegistration()
				.Register(new[] { 10, 42 })
				.ForAttendee("First name last name")
				.WithEmail("foo@bar.com")
				.SendConfirmationToAttendee()
				.WithFormOfAddress("Mr")
				.Build();
		}

		[Test]
		public void It_should_return_one_error()
		{
			Assert.AreEqual(1, Notifications.Count());
		}

		[Test]
		public void It_should_indicate_that_the_occupation_is_missing()
		{
			Assert.AreEqual("Please select your occupation.", Notifications.First().Message);
		}
	}

	public abstract class With_event_registration_validator : With_validator_for<IEventRegistrationCommand>
	{
		protected override IValidator<IEventRegistrationCommand> CreateValidator()
		{
			return new EventRegistrationCommandValidator();
		}
	}
}