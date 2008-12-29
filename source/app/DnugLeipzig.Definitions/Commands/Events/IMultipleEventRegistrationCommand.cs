using System.Collections.Generic;

namespace DnugLeipzig.Definitions.Commands.Events
{
	public interface IMultipleEventRegistrationCommand : IEventRegistrationCommand
	{
		IEnumerable<int> EventsToRegister
		{
			get;
		}

		void Initialize(IEnumerable<int> eventsToRegister,
		                                string name,
		                                string formOfAddress,
		                                string occupation,
		                                string attendeeEmail,
		                                bool sendConfirmationToAttendee);
	}
}