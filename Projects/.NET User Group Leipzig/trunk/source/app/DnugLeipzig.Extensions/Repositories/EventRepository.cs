using System;
using System.Collections.Generic;

using DnugLeipzig.Extensions.Filters;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Repositories
{
	public class EventRepository : PostRepository
	{
		public EventRepository(string categoryName) : base(categoryName)
		{
		}

		public override List<Post> Get(IPostFilter[] filters)
		{
			PostCollection posts = PostsByCategoryDisableHomepageOverride(int.MaxValue);

			List<Post> result = posts;
			foreach (IPostFilter filter in filters)
			{
				result = filter.Execute(result);
			}

			return result;
		}
	}
}