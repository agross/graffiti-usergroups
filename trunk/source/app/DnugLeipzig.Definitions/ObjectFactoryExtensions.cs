using DnugLeipzig.Definitions.Builders;

namespace DnugLeipzig.Definitions
{
	public static class ObjectFactoryExtensions
	{
		public static LogMessageBuilder LogMessage(this ObjectFactory ignored)
		{
			return new LogMessageBuilder();
		}
	}
}