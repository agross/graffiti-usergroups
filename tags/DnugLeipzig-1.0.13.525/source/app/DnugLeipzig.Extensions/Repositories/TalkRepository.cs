using System;
using System.Collections.Generic;

using DnugLeipzig.Extensions.Filters;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Repositories
{
	public class TalkRepository : IRepository<Post>
	{
		static readonly Data Data = new Data();
		readonly string _categoryName;
		readonly string _dateFieldName;

		public TalkRepository(string categoryName, string dateFieldName)
		{
			if (String.IsNullOrEmpty(categoryName))
			{
				throw new ArgumentOutOfRangeException("categoryName");
			}

			if (String.IsNullOrEmpty(dateFieldName))
			{
				throw new ArgumentOutOfRangeException("dateFieldName");
			}

			_categoryName = categoryName;
			_dateFieldName = dateFieldName;
		}

		#region IRepository<PostCollection> Members
		public List<Post> Get(params IPostFilter[] filters)
		{
			PostCollection posts = Data.PostsByCategory(_categoryName, int.MaxValue);

			// Pre-filter dates.
			HasDateFilter hasDate = new HasDateFilter(_dateFieldName);
			List<Post> result = hasDate.Execute(posts);

			foreach (IPostFilter filter in filters)
			{
				result = filter.Execute(result);
			}

			return result;
		}

		public Post Get(int id)
		{
			return Data.GetPost(id);
		}
		#endregion
	}
}