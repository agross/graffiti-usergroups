namespace DnugLeipzig.Definitions.Commands.Events
{
	public interface ISingleEventRegistrationCommand : IEventRegistrationCommand
	{
		int EventToRegister
		{
			get;
		}

		void Initialize(int eventToRegister,
		                                string name,
		                                string formOfAddress,
		                                string occupation,
		                                string attendeeEmail,
		                                bool sendConfirmationToAttendee);
	}
}