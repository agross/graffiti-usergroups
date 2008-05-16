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
		                                                      out ISettingsRepository settingsRepository,
		                                                      out IPostRepository postRepository)
		{
			categoryRepository = mocks.CreateMock<ICategoryRepository>();
			settingsRepository = mocks.CreateMock<ISettingsRepository>();
			postRepository = mocks.CreateMock<IPostRepository>();

			EventPlugin plugin = new EventPlugin(categoryRepository, postRepository, settingsRepository);
			plugin.CategoryName = EventCategoryName;

			return plugin;
		}
	}
}