using System.Collections.Generic;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Commands;

namespace DnugLeipzig.Runtime.Commands
{
	public class CommandFactory : ICommandFactory
	{
		#region Implementation of ICommandFactory
		public IEventRegistrationCommand EventRegistration(IEnumerable<int> eventsToRegister,
		                                                   string name,
		                                                   string attendeeEmail,
		                                                   bool sendConfirmationToAttendee)
		{
			var command = IoC.Resolve<IEventRegistrationCommand>();
			command.Initialize(eventsToRegister, name, attendeeEmail, sendConfirmationToAttendee);

			return command;
		}
		#endregion
	}
}