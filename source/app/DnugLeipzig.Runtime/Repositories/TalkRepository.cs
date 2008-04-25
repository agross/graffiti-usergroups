using DnugLeipzig.Definitions.Configuration;

namespace DnugLeipzig.Runtime.Repositories
{
	public class TalkRepository : CategoryEnabledRepository
	{
		public TalkRepository(ICategoryEnabledRepositoryConfigurationSource configuration) : base(configuration)
		{
		}
	}
}