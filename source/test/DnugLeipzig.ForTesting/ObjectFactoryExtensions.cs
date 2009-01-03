using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Configuration.Plugins;
using DnugLeipzig.ForTesting.Builders;

namespace DnugLeipzig.ForTesting
{
	public static class ObjectFactoryExtensions
	{
		public static EventBuilder Event(this ObjectFactory ignored, IEventPluginConfigurationProvider configurationProvider)
		{
			return new EventBuilder(configurationProvider);
		}

		public static EventRegistrationCommandBuilder EventRegistration(this ObjectFactory ignored)
		{
			return new EventRegistrationCommandBuilder();
		}

		public static StubbedEventPluginConfigurationBuilder StubbedEventPluginConfiguration(this ObjectFactory ignored)
		{
			return new StubbedEventPluginConfigurationBuilder();
		}
	}
}