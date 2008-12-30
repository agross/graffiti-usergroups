using DnugLeipzig.Definitions.Commands;

namespace DnugLeipzig.Definitions.Services
{
	public interface IEventRegistrationService
	{
		ICommandResult RegisterForEvents(IEventRegistrationCommand command);
	}
}