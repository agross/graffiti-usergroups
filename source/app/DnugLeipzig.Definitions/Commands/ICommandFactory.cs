using System.Collections.Generic;

namespace DnugLeipzig.Definitions.Commands
{
	public interface ICommandFactory
	{
		IEventRegistrationCommand EventRegistration(IEnumerable<int> eventsToRegister,
		                                            string name,
		                                            string attendeeEmail,
		                                            bool sendConfirmationToAttendee);
	}
}