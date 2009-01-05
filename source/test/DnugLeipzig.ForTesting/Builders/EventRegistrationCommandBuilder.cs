using System.Collections.Generic;

using DnugLeipzig.Definitions.Builders;
using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Definitions.Services;
using DnugLeipzig.Definitions.Validation;
using DnugLeipzig.Runtime.Commands;

namespace DnugLeipzig.ForTesting.Builders
{
	public class EventRegistrationCommandBuilder : EntityBuilder<EventRegistrationCommand>
	{
		string _attendeeEmail;
		IEnumerable<int> _eventsToRegister;
		string _formOfAddress;
		string _name;
		string _occupation;
		bool _sendConfirmationToAttendee;
		IEventRegistrationService _service;
		IValidator<IEventRegistrationCommand> _validator;

		protected override EventRegistrationCommand BuildInstance()
		{
			EventRegistrationCommand result = new EventRegistrationCommand(_validator, _service);
			result.Initialize(_eventsToRegister, _name, _formOfAddress, _occupation, _attendeeEmail, _sendConfirmationToAttendee);

			return result;
		}

		public static implicit operator EventRegistrationCommand(EventRegistrationCommandBuilder builder)
		{
			return builder.BuildInstance();
		}

		public EventRegistrationCommandBuilder SendConfirmationToAttendee()
		{
			_sendConfirmationToAttendee = true;
			return this;
		}

		public EventRegistrationCommandBuilder ForAttendee(string name)
		{
			_name = name;
			return this;
		}

		public EventRegistrationCommandBuilder WithFormOfAddress(string formOfAddress)
		{
			_formOfAddress = formOfAddress;
			return this;
		}

		public EventRegistrationCommandBuilder WithOccupation(string occupation)
		{
			_occupation = occupation;
			return this;
		}

		public EventRegistrationCommandBuilder WithEmail(string attendeeEmail)
		{
			_attendeeEmail = attendeeEmail;
			return this;
		}

		public EventRegistrationCommandBuilder ExecutedBy(IEventRegistrationService service)
		{
			_service = service;
			return this;
		}

		public EventRegistrationCommandBuilder Register(IEnumerable<int> eventsToRegister)
		{
			_eventsToRegister = eventsToRegister;
			return this;
		}

		public EventRegistrationCommandBuilder ValidatedBy(IValidator<IEventRegistrationCommand> validator)
		{
			_validator = validator;
			return this;
		}
	}
}