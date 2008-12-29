using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Definitions.Commands.Events;
using DnugLeipzig.Definitions.Services;

namespace DnugLeipzig.Runtime.Commands.Events
{
	public class SingleEventRegistrationCommand : EventRegistrationCommand, ISingleEventRegistrationCommand
	{
		public SingleEventRegistrationCommand(IEventRegistrationService eventRegistrationService)
			: base(eventRegistrationService)
		{
		}

		#region ISingleEventRegistrationCommand Members
		public void Initialize(int eventToRegister,
		                       string name,
		                       string formOfAddress,
		                       string occupation,
		                       string attendeeEmail,
		                       bool sendConfirmationToAttendee)
		{
			Initialize(name,
			           formOfAddress,
			           occupation,
			           attendeeEmail,
			           sendConfirmationToAttendee);

			EventToRegister = eventToRegister;
		}

		public int EventToRegister
		{
			get;
			protected set;
		}
		#endregion

		#region Implementation of ICommand
		public override ICommandResult Execute()
		{
			return EventRegistrationService.RegisterForEvent(this);
		}
		#endregion
	}
}