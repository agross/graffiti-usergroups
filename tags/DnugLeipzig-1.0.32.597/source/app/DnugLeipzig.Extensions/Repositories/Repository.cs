using System;
using System.Collections.Generic;

using DnugLeipzig.Extensions.Configuration;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Repositories
{
	public abstract class Repository : IRepository<Post>
	{
		protected static readonly Data Data = new Data();
		protected readonly IRepositoryConfigurationSource Configuration;

		public Repository(IRepositoryConfigurationSource configuration)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}

			Configuration = configuration;
		}

		#region IRepository<Post> Members
		Data IRepository<Post>.Data
		{
			get { return Data; }
		}

		public abstract IList<Post> GetAll();

		public virtual Post GetById(int id)
		{
			return Data.GetPost(id);
		}

		public virtual Category GetCategory()
		{
			return Data.GetCategory(Configuration.CategoryName);
		}
		#endregion

		protected IList<Post> PostsByCategoryDisableHomepageOverride(int count)
		{
			// HACK
			// Temporarily disable homepage overrides to get all posts of the category, even if they aren't
			// displayed on the home page. This is useful for general "overview" teasers that show all content
			// independent of the page.
			bool useCustomHomeList = Data.Site.UseCustomHomeList;
			Data.Site.UseCustomHomeList = false;
			PostCollection posts = Data.PostsByCategory(Configuration.CategoryName, count);
			Data.Site.UseCustomHomeList = useCustomHomeList;

			return posts;
		}
	}
}