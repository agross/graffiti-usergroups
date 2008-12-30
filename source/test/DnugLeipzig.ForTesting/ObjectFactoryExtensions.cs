using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Configuration.Plugins;
using DnugLeipzig.ForTesting.Builder;

namespace DnugLeipzig.ForTesting
{
	public static class ObjectFactoryExtensions
	{
		public static EventBuilder Event(this ObjectFactory ignored, IEventPluginConfiguration configuration)
		{
			return new EventBuilder(configuration);
		}
	}
}