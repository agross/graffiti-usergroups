using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Definitions.Commands.Events;
using DnugLeipzig.Definitions.Services;

namespace DnugLeipzig.Runtime.Commands.Events
{
	public abstract class EventRegistrationCommand : Command, IEventRegistrationCommand
	{
		protected EventRegistrationCommand(IEventRegistrationService eventRegistrationService)
		{
			EventRegistrationService = eventRegistrationService;
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
		#endregion

		protected void Initialize(string name,
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
		}
	}
}