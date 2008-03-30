using System;
using System.Collections.Generic;

using DnugLeipzig.Extensions.Filters;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Repositories
{
	public class EventRepository : IRepository<Post>
	{
		static readonly Data Data = new Data();
		readonly string _categoryName;
		PostCollection posts;

		public EventRepository(string categoryName)
		{
			if (String.IsNullOrEmpty(categoryName))
			{
				throw new ArgumentOutOfRangeException("categoryName");
			}

			_categoryName = categoryName;
		}

		#region IRepository<Post> Members
		public List<Post> Get(params IPostFilter[] filters)
		{
			posts = Data.PostsByCategory(_categoryName, int.MaxValue);

			List<Post> result = posts;
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