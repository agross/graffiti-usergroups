using System;
using System.Web;

using DnugLeipzig.Extensions.Configuration;
using DnugLeipzig.Extensions.DataObjects;
using DnugLeipzig.Extensions.Macros;
using DnugLeipzig.Extensions.Repositories;

using Graffiti.Core;

namespace DnugLeipzig.Extensions
{
	public class CalendarHandler : IHttpHandler
	{
		readonly IEventConfigurationSource Configuration;
		readonly IRepository<Post> Repository;

		public CalendarHandler() : this(null, new EventPluginConfigurationSource())
		{
		}

		public CalendarHandler(IRepository<Post> repository, IEventConfigurationSource configuration)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}

			Configuration = configuration;

			if (repository == null)
			{
				repository = new EventRepository(Configuration);
			}

			Repository = repository;
		}

		#region IHttpHandler Members
		public void ProcessRequest(HttpContext context)
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
				post = Repository.GetById(eventId);
			}
			catch
			{
				HttpContext.Current.Response.StatusCode = 404;
				HttpContext.Current.Response.End();
				return;
			}

			var events = new EventMacros(Repository, Configuration);

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
			context.Response.End();
		}

		public bool IsReusable
		{
			get { return true; }
		}
		#endregion
	}
}