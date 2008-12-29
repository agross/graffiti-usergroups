using System.Collections.Generic;

using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Definitions.Commands.Events;
using DnugLeipzig.Definitions.Services;

namespace DnugLeipzig.Runtime.Commands.Events
{
	public class MultipleEventRegistrationCommand : EventRegistrationCommand, IMultipleEventRegistrationCommand
	{
		public MultipleEventRegistrationCommand(IEventRegistrationService eventRegistrationService)
			: base(eventRegistrationService)
		{
		}

		#region IMultipleEventRegistrationCommand Members
		public void Initialize(IEnumerable<int> eventsToRegister,
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

			EventsToRegister = eventsToRegister;
		}

		public IEnumerable<int> EventsToRegister
		{
			get;
			protected set;
		}
		#endregion

		#region Implementation of ICommand
		public override ICommandResult Execute()
		{
			return EventRegistrationService.RegisterForEvents(this);
		}
		#endregion
	}
}