using System.Collections.Generic;

using DnugLeipzig.Extensions.Configuration;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Repositories
{
	public class EventRepository : Repository
	{
		public EventRepository(IRepositoryConfigurationSource configuration) : base(configuration)
		{
		}

		public override IList<Post> GetAll()
		{
			return PostsByCategoryDisableHomepageOverride(int.MaxValue);
		}
	}
}