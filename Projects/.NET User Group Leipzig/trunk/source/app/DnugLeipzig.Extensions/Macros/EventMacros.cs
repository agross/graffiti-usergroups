using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DnugLeipzig.Definitions.Configuration;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Extensions.DataObjects;
using DnugLeipzig.Extensions.Extensions;
using DnugLeipzig.Runtime.Repositories;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Macros
{
	[Chalk("events")]
	public class EventMacros : Macros
	{
		readonly IEventPluginConfigurationSource Configuration;

		#region ctors
		public EventMacros() : this(null, new EventPluginConfigurationSource())
		{
		}

		public EventMacros(ICategoryEnabledRepository repository, IEventPluginConfigurationSource configuration) : base(configuration)
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
		#endregion

		#region Dates, Location
		public string StartDate(Post post)
		{
			if (!post.Custom(Configuration.StartDateField).IsDate())
			{
				return HttpUtility.HtmlEncode(Configuration.UnknownText);
			}

			string dateFormat = Configuration.DateFormat;
			if (String.IsNullOrEmpty(dateFormat))
			{
				dateFormat = String.Format("{{0:{0}}}", SiteSettings.DateFormat);
			}

			return HttpUtility.HtmlEncode(String.Format(dateFormat, post.Custom(Configuration.StartDateField).AsEventDate()));
		}

		public string EndDate(Post post)
		{
			if (!post.Custom(Configuration.EndDateField).IsDate())
			{
				return HttpUtility.HtmlEncode(Configuration.UnknownText);
			}

			DateTime beginDate = post.Custom(Configuration.StartDateField).AsEventDate();
			DateTime endDate = post.Custom(Configuration.EndDateField).AsEventDate();

			string dateFormat = Configuration.DateFormat;
			if (beginDate.Date == endDate.Date && !String.IsNullOrEmpty(Configuration.ShortEndDateFormat))
			{
				dateFormat = Configuration.ShortEndDateFormat;
			}
			if (String.IsNullOrEmpty(dateFormat))
			{
				dateFormat = String.Format("{{0:{0}}}", SiteSettings.DateFormat);
			}

			return HttpUtility.HtmlEncode(String.Format(dateFormat, endDate));
		}

		public string Location(Post post)
		{
			string location = post.Custom(Configuration.LocationField);
			if (String.IsNullOrEmpty(location))
			{
				location = Configuration.UnknownText;
			}
			return HttpUtility.HtmlEncode(location);
		}

		public override string Speaker(Post post)
		{
			return Speaker(post, Configuration.UnknownText, null, null);
		}
		#endregion

		public IList<Post> GetForFuture()
		{
			return
				Repository.GetAll().IsInFuture(Configuration.StartDateField).SortAscending(Configuration.StartDateField).ToList();
		}

		public IList<Post> GetUpcoming(int numberOfEvents)
		{
			return
				Repository.GetAll().HasDate(Configuration.StartDateField).IsInFuture(Configuration.StartDateField).SortAscending(
					Configuration.StartDateField).LimitTo(numberOfEvents).ToList();
		}

		public IList<Post> GetForYear(int year)
		{
			return
				Repository.GetAll().IsInYear(Configuration.StartDateField, new DateTime(year, 1, 1)).IsInPast(
					Configuration.StartDateField).SortAscending(Configuration.StartDateField).ToList();
		}

		public ICollection<PastPostInfo> GetPastYearOverview()
		{
			IEnumerable<Post> posts = Repository.GetAll().IsInPast(Configuration.StartDateField);

			IEnumerable<PastPostInfo> pastEvents = from post in posts
			                                       group post by post.Custom(Configuration.StartDateField).AsEventDate().Year
			                                       into years orderby years.Key descending
			                                       	select
			                                       	new PastPostInfo
			                                       	{
			                                       		Year = years.Key,
			                                       		Url = Util.GetUrlForYearView(years.Key, Configuration.YearQueryString)
			                                       	};

			return pastEvents.ToList();
		}

		public bool CanCreateCalendarItem(Post post)
		{
			CalendarItem item = CreateCalendarItem(post);
			return item.IsValid();
		}

		public CalendarItem CreateCalendarItem(Post post)
		{
			return new CalendarItem
			       {
			       	StartDate = post.Custom(Configuration.StartDateField).AsEventDate(),
			       	EndDate = post.Custom(Configuration.EndDateField).AsEventDate(),
			       	Location = post.Custom(Configuration.LocationField),
			       	Subject = HttpUtility.HtmlDecode(post.Title),
			       	Description = SiteSettings.BaseUrl + post.Url,
			       	LastModified = post.Published,
			       	Categories = HttpUtility.HtmlDecode(Repository.GraffitiData.Site.Title)
			       };
		}
	}
}