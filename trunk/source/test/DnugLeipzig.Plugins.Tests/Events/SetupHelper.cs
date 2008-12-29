using DnugLeipzig.Definitions.Configuration;
using DnugLeipzig.Definitions.Repositories;

using Rhino.Mocks;

namespace DnugLeipzig.Plugins.Tests.Events
{
	internal static class SetupHelper
	{
		public const int EventCategoryId = int.MaxValue;
		public const string EventCategoryName = "Event category";

		public static EventPlugin SetUpWithMockedDependencies(MockRepository mocks,
		                                                      out ICategoryRepository categoryRepository,
		                                                      out IGraffitiSettings settings,
		                                                      out IPostRepository postRepository)
		{
			categoryRepository = mocks.StrictMock<ICategoryRepository>();
			settings = mocks.StrictMock<IGraffitiSettings>();
			postRepository = mocks.StrictMock<IPostRepository>();

			EventPlugin plugin = new EventPlugin(categoryRepository, postRepository, settings)
			                     { CategoryName = EventCategoryName };

			return plugin;
		}
	}
}