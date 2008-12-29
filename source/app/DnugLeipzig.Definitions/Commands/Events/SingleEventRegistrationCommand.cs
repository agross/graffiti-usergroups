using DnugLeipzig.Definitions.Services;

namespace DnugLeipzig.Definitions.Commands.Events
{
	public class SingleEventRegistrationCommand : EventRegistrationCommand
	{
		public SingleEventRegistrationCommand(int eventToRegister,
		                                      string name,
		                                      string formOfAddress,
		                                      string occupation,
		                                      string attendeeEmail,
		                                      bool sendConfirmationToAttendee)
		{
			EventToRegister = eventToRegister;

			Name = name;
			FormOfAddress = formOfAddress;
			Occupation = occupation;
			AttendeeEmail = attendeeEmail;
			SendConfirmationToAttendee = sendConfirmationToAttendee;
		}

		#region Implementation of ICommand
		public override ICommandResult Execute()
		{
			return IoC.Resolve<IEventRegistrationService>().RegisterForEvent(this);
		}
		#endregion

		public int EventToRegister
		{
			get;
			protected set;
		}
	}
}