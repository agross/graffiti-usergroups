using System;
using System.Web;

using Castle.Core.Logging;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Definitions.Commands.Results;
using DnugLeipzig.Definitions.Repositories;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Handlers
{
	public class CalendarHandler : IHttpHandler
	{
		readonly IPostRepository _postRepository;
		readonly ICalendarItemRepository _calendarItemRepository;
		ILogger _logger;

		public CalendarHandler() : this(IoC.Resolve<IPostRepository>(), IoC.Resolve<ICalendarItemRepository>(), IoC.Resolve<ILogger>())
		{
		}

		public CalendarHandler(IPostRepository postRepository, ICalendarItemRepository calendarItemRepository, ILogger logger)
		{
			_postRepository = postRepository;
			_calendarItemRepository = calendarItemRepository;
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
			if (context.Request.RequestType != "GET")
			{
				new ForbiddenResult().Render(context.Response);
				return;
			}

			int eventId;
			if (!int.TryParse(context.Request.QueryString["eventId"], out eventId))
			{
				new NotFoundResult().Render(context.Response);
				return;
			}

			try
			{
				Post post = _postRepository.GetById(eventId);
				if (post == null)
				{
					new NotFoundResult().Render(context.Response);
					return;
				}

				ICalendarItem item = _calendarItemRepository.CreateCalendarItemForEvent(post);
				item.Render(context.Response);
			}
			catch (Exception ex)
			{
				Logger.Error(Create.New.LogMessage().WithTitle("Could not generate calendar item"), ex);
				new ErrorResult().Render(context.Response);
			}
		}

		public bool IsReusable
		{
			get { return true; }
		}
		#endregion
	}
}