using System;
using System.Web;

using DnugLeipzig.Definitions.Configuration;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Extensions.DataObjects;
using DnugLeipzig.Extensions.Macros;
using DnugLeipzig.Runtime.Repositories;

using Graffiti.Core;

using EventPluginConfiguration=DnugLeipzig.Extensions.Configuration.EventPluginConfiguration;

namespace DnugLeipzig.Extensions.Handlers
{
	public class CalendarHandler : IHttpHandler
	{
		readonly IEventPluginConfiguration _configuration;
		readonly ICategoryEnabledRepository _repository;

		#region Ctors
		public CalendarHandler() : this(null, new EventPluginConfiguration())
		{
		}

		public CalendarHandler(ICategoryEnabledRepository repository, IEventPluginConfiguration configuration)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}

			_configuration = configuration;

			if (repository == null)
			{
				repository = new EventRepository(_configuration);
			}

			_repository = repository;
		}
		#endregion

		#region IHttpHandler Members
		public void ProcessRequest(HttpContext context)
		{
			try
			{
				int eventId;
				if (!int.TryParse(context.Request.QueryString["eventId"], out eventId))
				{
					HttpContext.Current.Response.StatusCode = 404;
					HttpContext.Current.Response.End();
					return;
				}

				Post post;
				try
				{
					post = _repository.GetById(eventId);
				}
				catch
				{
					HttpContext.Current.Response.StatusCode = 404;
					HttpContext.Current.Response.End();
					return;
				}

				var events = new EventMacros(_repository, _configuration);

				if (post == null || !events.CanCreateCalendarItem(post))
				{
					HttpContext.Current.Response.StatusCode = 404;
					HttpContext.Current.Response.End();
					return;
				}

				CalendarItem item = events.CreateCalendarItem(post);
				string serializedItem = item.ToString();

				context.Response.Clear();
				context.Response.AppendHeader("Content-Disposition",
				                              String.Format("attachment; filename={0}.ics",
				                                            HttpUtility.UrlPathEncode(HttpUtility.HtmlDecode(post.Title))));
				context.Response.AppendHeader("Content-Length", serializedItem.Length.ToString());
				context.Response.ContentType = "text/calendar";
				context.Response.Write(serializedItem);
			}
			catch (Exception ex)
			{
				Log.Error("Could not generate calendar item", ex.ToString());
				context.Response.Clear();
				throw;
			}
		}

		public bool IsReusable
		{
			get { return true; }
		}
		#endregion
	}
}