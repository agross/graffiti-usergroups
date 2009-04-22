using System;
using System.Web;

using Castle.Core.Logging;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Commands.Results;
using DnugLeipzig.Definitions.Plugins.Events;
using DnugLeipzig.Definitions.Repositories;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Handlers
{
	public class CalendarHandler : IHttpHandler
	{
		readonly ICalendarItemRepository _calendarItemRepository;
		readonly ICategorizedPostRepository<IEventPluginConfigurationProvider> _eventRepository;
		ILogger _logger;

		public CalendarHandler() : this(IoC.Resolve<ICategorizedPostRepository<IEventPluginConfigurationProvider>>(),
		                                IoC.Resolve<ICalendarItemRepository>(),
		                                IoC.Resolve<ILogger>())
		{
		}

		public CalendarHandler(ICategorizedPostRepository<IEventPluginConfigurationProvider> postRepository,
		                       ICalendarItemRepository calendarItemRepository,
		                       ILogger logger)
		{
			_eventRepository = postRepository;
			_calendarItemRepository = calendarItemRepository;
			Logger = logger;
		}

		ILogger Logger
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

			if (String.IsNullOrEmpty(context.Request.QueryString["eventId"]))
			{
				CreateCalendar(context);
				return;
			}

			CreateCalendarItem(context);
		}

		public bool IsReusable
		{
			get { return true; }
		}
		#endregion

		void CreateCalendar(HttpContext context)
		{
			try
			{
				var posts = _eventRepository.GetAll();
				var calendar = _calendarItemRepository.CreateCalendar(posts);
				calendar.Render(context.Response);
			}
			catch (Exception ex)
			{
				Logger.Error(Create.New.LogMessage().WithTitle("Could not generate calendar"), ex);
				new ErrorResult().Render(context.Response);
			}
		}

		void CreateCalendarItem(HttpContext context)
		{
			int eventId;
			if (!int.TryParse(context.Request.QueryString["eventId"], out eventId))
			{
				new NotFoundResult().Render(context.Response);
				return;
			}

			try
			{
				Post post = _eventRepository.GetById(eventId);
				if (post == null)
				{
					new NotFoundResult().Render(context.Response);
					return;
				}

				ICalendar calendar = _calendarItemRepository.CreateCalendarForEvent(post);
				calendar.Render(context.Response);
			}
			catch (Exception ex)
			{
				Logger.Error(Create.New.LogMessage().WithTitle("Could not generate calendar item"), ex);
				new ErrorResult().Render(context.Response);
			}
		}
	}
}