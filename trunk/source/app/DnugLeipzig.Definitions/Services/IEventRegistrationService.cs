using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Definitions.Commands.Events;

namespace DnugLeipzig.Definitions.Services
{
	public interface IEventRegistrationService
	{
		ICommandResult RegisterForEvents(IMultipleEventRegistrationCommand command);
		ICommandResult RegisterForEvent(ISingleEventRegistrationCommand command);
	}
}