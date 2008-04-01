using System;
using System.Configuration;
using System.Web;

using DnugLeipzig.Extensions.DataObjects;
using DnugLeipzig.Extensions.Extensions;
using DnugLeipzig.Extensions.Repositories;

using Graffiti.Core;

using Events=DnugLeipzig.Extensions.Macros.Events;

namespace DnugLeipzig.Extensions
{
	public class CalendarHandler : IHttpHandler
	{
		static readonly string CategoryName;
		static readonly string BeginDateFieldName;
		static string LocationFieldName;
		readonly IRepository<Post> Repository;
		static string EndDateFieldName;

		static CalendarHandler()
		{
			CategoryName = ConfigurationManager.AppSettings.GetOrDefault("UserGroup:Talks:CategoryName", "Vortr&#228;ge");
			BeginDateFieldName = ConfigurationManager.AppSettings.GetOrDefault("UserGroup:Talks:BeginDateFieldName", "Datum Anfang");
			EndDateFieldName = ConfigurationManager.AppSettings.GetOrDefault("UserGroup:Talks:EndDateFieldName", "Datum Anfang");
			LocationFieldName = ConfigurationManager.AppSettings.GetOrDefault("UserGroup:Events:LocationFieldName", "Ort");
		}

		public CalendarHandler() : this(null)
		{
		}

		public CalendarHandler(IRepository<Post> repository)
		{
			if (repository == null)
			{
				repository = new EventRepository(CategoryName);
			}

			Repository = repository;
		}

		#region IHttpHandler Members
		public void ProcessRequest(HttpContext context)
		{
			int eventId;
			if(!int.TryParse(context.Request.QueryString["eventId"], out eventId))
			{
				HttpContext.Current.Response.StatusCode = 404;
				HttpContext.Current.Response.End();
				return;
			}

			Post post;
			try
			{
				post = Repository.Get(eventId);
			}
			catch
			{
				HttpContext.Current.Response.StatusCode = 404;
				HttpContext.Current.Response.End();
				return;
			}

			Events events = new Events();

			if (post == null || !events.CanCreateCalendarItem(post))
			{
				HttpContext.Current.Response.StatusCode = 404;
				HttpContext.Current.Response.End();
				return;
			}

			CalendarItem item = events.CreateCalendarItem(post);
			string serializedItem = item.ToString();

			context.Response.Clear();
			context.Response.AppendHeader("Content-Disposition", String.Format("attachment; filename={0}.ics", HttpUtility.UrlPathEncode(HttpUtility.HtmlDecode(post.Title))));
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