using Castle.Core.Logging;

namespace DnugLeipzig.Runtime.Services
{
	public abstract class Service
	{
		ILogger _logger;

		public ILogger Logger
		{
			get
			{
				if (_logger == null)
				{
					return NullLogger.Instance;
				}
				return _logger;
			}
			set { _logger = value; }
		}
	}
}