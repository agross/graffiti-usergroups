using System;
using System.Collections.Generic;

using DnugLeipzig.Extensions.Configuration;
using DnugLeipzig.Extensions.Filters;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Repositories
{
	public class TalkRepository : Repository
	{
		public TalkRepository(IRepositoryConfigurationSource configuration) : base(configuration)
		{
		}

		public override List<Post> Get(IPostFilter[] filters)
		{
			PostCollection posts = PostsByCategoryDisableHomepageOverride(int.MaxValue);

			// Pre-filter dates.
			var hasDate = new HasDate(Configuration.SortRelevantDateField);
			List<Post> result = hasDate.Execute(posts);

			foreach (IPostFilter filter in filters)
			{
				result = filter.Execute(result);
			}

			return result;
		}
	}
}