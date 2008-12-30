using DnugLeipzig.Definitions.Commands;

namespace DnugLeipzig.Definitions.Services
{
	public interface IEventRegistrationService
	{
		IHttpResponse RegisterForEvents(IEventRegistrationCommand command);
	}
}