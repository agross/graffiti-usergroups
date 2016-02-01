using System.Collections.Generic;

namespace DnugLeipzig.Definitions.Commands
{
	public interface IEventRegistrationCommand : ICommand
	{
		string Name
		{
			get;
		}

		string AttendeeEmail
		{
			get;
		}

		bool SendConfirmationToAttendee
		{
			get;
		}

		IEnumerable<int> EventsToRegister
		{
			get;
		}

		void Initialize(IEnumerable<int> eventsToRegister,
		                string name,
		                string attendeeEmail,
		                bool sendConfirmationToAttendee);
	}
}