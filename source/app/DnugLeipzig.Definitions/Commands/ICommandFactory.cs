using System.Collections.Generic;

namespace DnugLeipzig.Definitions.Commands
{
	public interface ICommandFactory
	{
		ICommand CreateCalendarItem(int eventId);

		IEventRegistrationCommand EventRegistration(IEnumerable<int> eventsToRegister, string formOfAddress, string name, string occupation, string attendeeEmail, bool sendConfirmationToAttendee);
	}
}