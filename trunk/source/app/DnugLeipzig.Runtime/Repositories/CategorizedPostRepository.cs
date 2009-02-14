using System.Collections.Generic;

using DnugLeipzig.Definitions.Plugins;
using DnugLeipzig.Definitions.Repositories;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Repositories
{
	public class CategorizedPostRepository<TConfiguration>
		: PostRepository, ICategorizedPostRepository<TConfiguration>
		where TConfiguration : IPluginConfigurationProvider
	{
		readonly TConfiguration _configuration;

		public CategorizedPostRepository(TConfiguration configuration)
		{
			_configuration = configuration;
		}

		#region ICategorizedPostRepository<TConfiguration> Members
		public TConfiguration Configuration
		{
			get { return _configuration; }
		}

		public IList<Post> GetAll()
		{
			return PostsByCategoryDisableHomepageOverride(int.MaxValue);
		}

		public Category GetCategory()
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
			PostCollection posts = Data.PostsByCategory(_configuration.CategoryName, count);
			Data.Site.UseCustomHomeList = useCustomHomeList;

			return posts;
		}
	}
}