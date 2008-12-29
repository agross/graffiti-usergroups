using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Definitions.Commands.Events;

namespace DnugLeipzig.Definitions.Services
{
	public interface IEventRegistrationService
	{
		ICommandResult RegisterForEvents(MultipleEventRegistrationCommand command);
		ICommandResult RegisterForEvent(SingleEventRegistrationCommand command);
	}
}