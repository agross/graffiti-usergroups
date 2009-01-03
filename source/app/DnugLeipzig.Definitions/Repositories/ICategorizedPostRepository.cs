using System.Collections.Generic;

using DnugLeipzig.Definitions.Configuration.Plugins;

using Graffiti.Core;

namespace DnugLeipzig.Definitions.Repositories
{
	public interface ICategorizedPostRepository<TConfiguration> : IPostRepository
		where TConfiguration : IPluginConfigurationProvider
	{
		TConfiguration Configuration
		{
			get;
		}

		IList<Post> GetAll();
		Category GetCategory();
	}
}