using System;
using System.Collections.Generic;

using DnugLeipzig.Extensions.Filters;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Repositories
{
	public class TalkRepository : PostRepository
	{
		readonly string _dateFieldName;

		public TalkRepository(string categoryName, string dateFieldName) : base(categoryName)
		{
			if (String.IsNullOrEmpty(dateFieldName))
			{
				throw new ArgumentOutOfRangeException("dateFieldName");
			}

			_dateFieldName = dateFieldName;
		}

		#region IRepository<Post> Members
		public override List<Post> Get(IPostFilter[] filters)
		{
			PostCollection posts = PostsByCategoryDisableHomepageOverride(int.MaxValue);

			// Pre-filter dates.
			var hasDate = new HasDate(_dateFieldName);
			List<Post> result = hasDate.Execute(posts);

			foreach (IPostFilter filter in filters)
			{
				result = filter.Execute(result);
			}

			return result;
		}
	
		#endregion
	}
}