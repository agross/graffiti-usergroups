using DnugLeipzig.Definitions;
using DnugLeipzig.ForTesting.Builders;

namespace DnugLeipzig.ForTesting
{
	public static class ObjectFactoryExtensions
	{
		public static EventBuilder Event(this ObjectFactory ignored)
		{
			return new EventBuilder(new StubbedEventPluginConfigurationBuilder().Build());
		}

		public static TalkBuilder Talk(this ObjectFactory ignored)
		{
			return new TalkBuilder(new StubbedTalkPluginConfigurationBuilder().Build());
		}

		public static EventRegistrationCommandBuilder EventRegistration(this ObjectFactory ignored)
		{
			return new EventRegistrationCommandBuilder();
		}

		public static StubbedEventPluginConfigurationBuilder StubbedEventPluginConfiguration(this ObjectFactory ignored)
		{
			return new StubbedEventPluginConfigurationBuilder();
		}

		public static StubbedTalkPluginConfigurationBuilder StubbedTalkPluginConfiguration(this ObjectFactory ignored)
		{
			return new StubbedTalkPluginConfigurationBuilder();
		}
	}
}