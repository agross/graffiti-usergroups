using System;
using System.Configuration;
using System.Web;

using DnugLeipzig.Extensions.DataObjects;
using DnugLeipzig.Extensions.Extensions;
using DnugLeipzig.Extensions.Repositories;

using Graffiti.Core;

namespace DnugLeipzig.Extensions
{
	public class CalendarHandler : IHttpHandler
	{
		static readonly string CategoryName;
		static readonly string DateFieldName;
		static string LocationFieldName;
		readonly IRepository<Post> Repository;

		static CalendarHandler()
		{
			CategoryName = ConfigurationManager.AppSettings.GetOrDefault("UserGroup:Talks:CategoryName", "Vortr&#228;ge");
			DateFieldName = ConfigurationManager.AppSettings.GetOrDefault("UserGroup:Talks:DateFieldName", "Datum");
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
				// TODO: Should we throw an exception?
				return;
			}

			Post @event = Repository.Get(eventId);

			if (!@event.Custom(DateFieldName).IsDate())
			{
				// No date for the event.
				return;
			}

			ICalendarItem calendarItem = new ICalendarItem();
			calendarItem.Location = @event.Custom(LocationFieldName);
			calendarItem.StartDate = @event.Custom(DateFieldName).AsEventDate();
			// HACK.
			calendarItem.EndDate = @event.Custom(DateFieldName).AsEventDate().AddHours(3);
			calendarItem.Subject = @event.Title;
			calendarItem.Description = @event.Url;
			calendarItem.LastModified = @event.Published;
			

			string icalItem = calendarItem.ToString();

			context.Response.Clear();
			context.Response.AppendHeader("Content-Disposition", "attachment; filename=Maintenance.vcs");
			context.Response.AppendHeader("Content-Length", icalItem.Length.ToString());
			context.Response.ContentType = "application/download";
			context.Response.Write(icalItem);
			context.Response.End();
		}

		public bool IsReusable
		{
			get { return true; }
		}
		#endregion
	}
}