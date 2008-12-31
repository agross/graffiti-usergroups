using DnugLeipzig.Definitions.Commands;

namespace DnugLeipzig.Definitions.Services
{
	public interface IEventRegistrationService
	{
		IEventRegistrationResultList RegisterForEvents(IEventRegistrationCommand command);
	}
}