using System.Collections.Generic;

using DnugLeipzig.Definitions.Services;

namespace DnugLeipzig.Definitions.Commands.Events
{
	public class MultipleEventRegistrationCommand : EventRegistrationCommand
	{
		public MultipleEventRegistrationCommand(IEnumerable<int> eventsToRegister,
		                                        string name,
		                                        string formOfAddress,
		                                        string occupation,
		                                        string attendeeEmail,
		                                        bool sendConfirmationToAttendee)
		{
			EventsToRegister = eventsToRegister;

			Name = name;
			FormOfAddress = formOfAddress;
			Occupation = occupation;
			AttendeeEmail = attendeeEmail;
			SendConfirmationToAttendee = sendConfirmationToAttendee;
		}

		#region Implementation of ICommand
		public override ICommandResult Execute()
		{
			return IoC.Resolve<IEventRegistrationService>().RegisterForEvents(this);
		}
		#endregion

		public IEnumerable<int> EventsToRegister
		{
			get;
			protected set;
		}
	}
}