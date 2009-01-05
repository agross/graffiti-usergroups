using System.Collections.Generic;
using System.Linq;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Definitions.Commands.Results;
using DnugLeipzig.Definitions.Services;
using DnugLeipzig.Definitions.Validation;

namespace DnugLeipzig.Runtime.Commands
{
	public class EventRegistrationCommand : Command, IEventRegistrationCommand
	{
		public EventRegistrationCommand(IValidator<IEventRegistrationCommand> validator,
		                                IEventRegistrationService eventRegistrationService)
		{
			Validator = validator;
			EventRegistrationService = eventRegistrationService;
		}

		protected IValidator<IEventRegistrationCommand> Validator
		{
			get;
			set;
		}

		protected IEventRegistrationService EventRegistrationService
		{
			get;
			private set;
		}

		#region IEventRegistrationCommand Members
		public string Name
		{
			get;
			protected set;
		}

		public string FormOfAddress
		{
			get;
			protected set;
		}

		public string Occupation
		{
			get;
			protected set;
		}

		public string AttendeeEmail
		{
			get;
			protected set;
		}

		public bool SendConfirmationToAttendee
		{
			get;
			protected set;
		}

		public void Initialize(IEnumerable<int> eventsToRegister,
		                       string name,
		                       string formOfAddress,
		                       string occupation,
		                       string attendeeEmail,
		                       bool sendConfirmationToAttendee)
		{
			Name = name;
			FormOfAddress = formOfAddress;
			Occupation = occupation;
			AttendeeEmail = attendeeEmail;
			SendConfirmationToAttendee = sendConfirmationToAttendee;

			EventsToRegister = eventsToRegister;
		}

		public IEnumerable<int> EventsToRegister
		{
			get;
			protected set;
		}
		#endregion

		#region Implementation of ICommand
		public override IHttpResponse Execute()
		{
			IEnumerable<INotification> validationErrors = Validator.Validate(this);
			if (validationErrors.Any())
			{
				return new ValidationErrorResult(validationErrors);
			}

			return EventRegistrationService.RegisterForEvents(this);
		}
		#endregion
	}
}