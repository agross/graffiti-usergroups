using DnugLeipzig.Definitions.Configuration;

namespace DnugLeipzig.Runtime.Repositories
{
	public class EventRepository : CategoryEnabledRepository
	{
		public EventRepository(ICategoryEnabledRepositoryConfiguration configuration) : base(configuration)
		{
		}
	}
}