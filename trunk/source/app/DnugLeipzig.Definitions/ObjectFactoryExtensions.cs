using DnugLeipzig.Definitions.Builders;

namespace DnugLeipzig.Definitions
{
	public static class ObjectFactoryExtensions
	{
		public static MessageBuilder Message(this ObjectFactory ignored)
		{
			return new MessageBuilder();
		}
	}
}