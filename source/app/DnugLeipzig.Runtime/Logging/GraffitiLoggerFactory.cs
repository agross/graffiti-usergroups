using Castle.Core.Logging;

namespace DnugLeipzig.Runtime.Logging
{
	public class GraffitiLoggerFactory : AbstractLoggerFactory
	{
		#region Overrides of AbstractLoggerFactory
		public override ILogger Create(string name)
		{
			return new GraffitiLogger(name);
		}

		public override ILogger Create(string name, LoggerLevel level)
		{
			return Create(name);
		}
		#endregion
	}
}