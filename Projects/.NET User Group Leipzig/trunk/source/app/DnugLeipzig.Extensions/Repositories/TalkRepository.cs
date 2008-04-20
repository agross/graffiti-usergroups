using System.Collections.Generic;

using DnugLeipzig.Extensions.Configuration;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Repositories
{
	public class TalkRepository : Repository
	{
		public TalkRepository(IRepositoryConfigurationSource configuration) : base(configuration)
		{
		}

		public override IList<Post> GetAll()
		{
			return PostsByCategoryDisableHomepageOverride(int.MaxValue);
		}
	}
}