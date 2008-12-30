using Castle.Core.Logging;

using DnugLeipzig.Definitions.Commands;

namespace DnugLeipzig.Runtime.Commands
{
	public abstract class Command : ICommand
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

		#region Implementation of ICommand
		public abstract ICommandResult Execute();
		#endregion
	}
}