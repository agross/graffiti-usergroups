using System;
using System.Web;

using Castle.Core.Logging;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Definitions.Commands.Results;

namespace DnugLeipzig.Runtime.Handlers
{
	public class CalendarHandler : IHttpHandler
	{
		readonly ICommandFactory _commandFactory;
		ILogger _logger;

		public CalendarHandler() : this(IoC.Resolve<ICommandFactory>(), IoC.Resolve<ILogger>())
		{
		}

		public CalendarHandler(ICommandFactory commandFactory, ILogger logger)
		{
			_commandFactory = commandFactory;
			Logger = logger;
		}

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

		#region IHttpHandler Members
		public void ProcessRequest(HttpContext context)
		{
			int eventId;
			if (!int.TryParse(context.Request.QueryString["eventId"], out eventId))
			{
				new NotFoundResult().Render(HttpContext.Current.Response);
				return;
			}

			try
			{
				var command = _commandFactory.CreateCalendarItem(eventId);
				ICommandResult result = command.Execute();

				result.Render(HttpContext.Current.Response);
			}
			catch (Exception ex)
			{
				Logger.Error(Create.New.Message().WithTitle("Could not generate calendar item"), ex);
				new ErrorResult().Render(HttpContext.Current.Response);
			}
		}

		public bool IsReusable
		{
			get { return true; }
		}
		#endregion
	}
}