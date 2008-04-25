using DnugLeipzig.Definitions.Configuration;

namespace DnugLeipzig.Runtime.Repositories
{
	public class EventRepository : CategoryEnabledRepository
	{
		public EventRepository(ICategoryEnabledRepositoryConfigurationSource configuration) : base(configuration)
		{
		}
	}
}