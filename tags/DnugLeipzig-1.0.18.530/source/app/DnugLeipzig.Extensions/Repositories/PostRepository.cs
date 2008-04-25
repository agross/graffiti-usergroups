using System;
using System.Collections.Generic;

using DnugLeipzig.Extensions.Filters;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Repositories
{
	public abstract class PostRepository:IRepository<Post>
	{
		protected readonly string _categoryName;
		protected static readonly Data Data = new Data();

		public PostRepository(string categoryName)
		{
			if (String.IsNullOrEmpty(categoryName))
			{
				throw new ArgumentOutOfRangeException("categoryName");
			}

			_categoryName = categoryName;
		}

		#region IRepository<Post> Members
		protected PostCollection PostsByCategoryDisableHomepageOverride(int count)
		{
			// HACK
			// Temporarily disable homepage overrides to get all posts of the category, even if they aren't
			// displayed on the home page. This is useful for general "overview" teasers that show all content
			// independent of the page.
			bool useCustomHomeList = Data.Site.UseCustomHomeList;
			Data.Site.UseCustomHomeList = false;
			PostCollection posts = Data.PostsByCategory(_categoryName, count);
			Data.Site.UseCustomHomeList = useCustomHomeList;

			return posts;
		}

		public abstract List<Post> Get(params IPostFilter[] filters);

		public virtual Post Get(int id)
		{
			return Data.GetPost(id);
		}

		public virtual Category GetCategory()
		{
			return Data.GetCategory(_categoryName);
		}
		#endregion
	}
}