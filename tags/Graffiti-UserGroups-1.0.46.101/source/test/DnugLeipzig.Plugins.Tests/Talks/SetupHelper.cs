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
		                                                      out ISettingsRepository settingsRepository,
		                                                      out IPostRepository postRepository)
		{
			categoryRepository = mocks.CreateMock<ICategoryRepository>();
			settingsRepository = mocks.CreateMock<ISettingsRepository>();
			postRepository = mocks.CreateMock<IPostRepository>();

			TalkPlugin plugin = new TalkPlugin(categoryRepository, postRepository);
			plugin.CategoryName = TalkCategoryName;

			return plugin;
		}
	}
}