using System;
using System.Collections.Generic;

using DnugLeipzig.Definitions.Configuration;
using DnugLeipzig.Definitions.Repositories;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Repositories
{
	public abstract class CategoryEnabledRepository : PostRepository, ICategoryEnabledRepository
	{
		protected readonly ICategoryEnabledRepositoryConfigurationSource Configuration;

		protected CategoryEnabledRepository(ICategoryEnabledRepositoryConfigurationSource configuration)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}

			Configuration = configuration;
		}

		#region ICategoryEnabledRepository Members
		public IList<Post> GetAll()
		{
			return PostsByCategoryDisableHomepageOverride(int.MaxValue);
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