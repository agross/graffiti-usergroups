using System.Collections.Generic;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Definitions.Commands.Results;
using DnugLeipzig.Definitions.Services;
using DnugLeipzig.Definitions.Validation;
using DnugLeipzig.ForTesting;
using DnugLeipzig.Runtime.Commands;
using DnugLeipzig.Runtime.Validation;

using MbUnit.Framework;

using Rhino.Mocks;

namespace DnugLeipzig.Runtime.Tests.Commands
{
	public class When_a_valid_event_registration_request_is_executed : With_event_registration_command
	{
		protected override void Establish_context()
		{
			base.Establish_context();

			Validator.Stub(x => x.Validate(Command)).Return(new NotificationResult());
		}

		[Test]
		public void It_should_validate_the_request()
		{
			Validator.AssertWasCalled(x => x.Validate(Command));
		}

		[Test]
		public void It_should_execute_the_command()
		{
			Service.AssertWasCalled(x => x.RegisterForEvents(Command));
		}
	}

	public class When_an_invalid_event_registration_request_is_executed : With_event_registration_command
	{
		protected override void Establish_context()
		{
			base.Establish_context();

			Validator.Stub(x => x.Validate(Command)).Return(new NotificationResult() { new ValidationError("Something does not validate") });
		}

		[Test]
		public void It_should_validate_the_request()
		{
			Validator.AssertWasCalled(x => x.Validate(Command));
		}

		[Test]
		public void It_should_return_the_validation_errors()
		{
			Assert.IsInstanceOfType(typeof(ValidationErrorResult), Response);
		}

		[Test]
		public void It_should_not_execute_the_command()
		{
			Service.AssertWasNotCalled(x => x.RegisterForEvents(null), o => o.IgnoreArguments());
		}
	}

	public abstract class With_event_registration_command : Spec
	{
		protected IHttpResponse Response
		{
			get;
			private set;
		}

		protected EventRegistrationCommand Command
		{
			get;
			private set;
		}

		protected IEventRegistrationService Service
		{
			get;
			private set;
		}

		protected IValidator<IEventRegistrationCommand> Validator
		{
			get;
			private set;
		}

		protected override void Establish_context()
		{
			Service = MockRepository.GenerateMock<IEventRegistrationService>();
			Validator = MockRepository.GenerateMock<IValidator<IEventRegistrationCommand>>();

			Command = Create.New.EventRegistration()
				.ExecutedBy(Service)
				.ValidatedBy(Validator);
		}

		protected override void Because()
		{
			Response = Command.Execute();
		}
	}
}