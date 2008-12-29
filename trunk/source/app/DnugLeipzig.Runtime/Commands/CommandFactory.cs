using System.Collections.Generic;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Definitions.Commands.Calendar;
using DnugLeipzig.Definitions.Commands.Events;
using DnugLeipzig.Runtime.Commands.Calendar;

namespace DnugLeipzig.Runtime.Commands
{
	public class CommandFactory:ICommandFactory
	{
		#region Implementation of ICommandFactory
		public ICommand CreateCalendarItem(int eventId)
		{
			var command = IoC.Resolve<ICreateCalendarItemCommand>();
//			command.Initialize(eventId);
			return command;
		}

		public ICommand MultipleEventRegistration(IEnumerable<int> eventsToRegister, string name, string formOfAddress, string occupation, string attendeeEmail, bool sendConfirmationToAttendee)
		{
			var command = IoC.Resolve<IMultipleEventRegistrationCommand>();
			command.Initialize(eventsToRegister, name, formOfAddress, occupation, attendeeEmail, sendConfirmationToAttendee);

			return command;
		}
		#endregion
	}
}