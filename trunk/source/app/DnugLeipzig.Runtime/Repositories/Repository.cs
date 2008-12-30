using Castle.Core.Logging;

namespace DnugLeipzig.Runtime.Repositories
{
	public abstract class Repository
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