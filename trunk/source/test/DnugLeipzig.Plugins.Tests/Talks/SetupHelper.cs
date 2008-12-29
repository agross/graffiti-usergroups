using DnugLeipzig.Definitions.Configuration;
using DnugLeipzig.Definitions.Repositories;

using Rhino.Mocks;

namespace DnugLeipzig.Plugins.Tests.Talks
{
	internal static class SetupHelper
	{
		public const int TalkCategoryId = int.MaxValue;
		public const string TalkCategoryName = "Talk category";

		public static TalkPlugin SetUpWithMockedDependencies(MockRepository mocks,
		                                                      out ICategoryRepository categoryRepository,
		                                                      out IGraffitiSettings settings,
		                                                      out IPostRepository postRepository)
		{
			categoryRepository = mocks.StrictMock<ICategoryRepository>();
			settings = mocks.StrictMock<IGraffitiSettings>();
			postRepository = mocks.StrictMock<IPostRepository>();

			TalkPlugin plugin = new TalkPlugin(categoryRepository, postRepository);
			plugin.CategoryName = TalkCategoryName;

			return plugin;
		}
	}
}