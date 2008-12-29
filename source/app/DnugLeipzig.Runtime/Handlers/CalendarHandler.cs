using System;
using System.Web;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Definitions.Commands.Results;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Handlers
{
	public class CalendarHandler : IHttpHandler
	{
		readonly ICommandFactory _commandFactory;

		public CalendarHandler() : this(IoC.Resolve<ICommandFactory>())
		{
		}

		public CalendarHandler(ICommandFactory commandFactory)
		{
			_commandFactory = commandFactory;
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
				Log.Error("Could not generate calendar item", ex.ToString());
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