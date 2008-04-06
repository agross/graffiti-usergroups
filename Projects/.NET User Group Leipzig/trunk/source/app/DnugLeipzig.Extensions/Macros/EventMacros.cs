using System;
using System.Collections.Generic;
using System.Linq;

using DnugLeipzig.Extensions.Configuration;
using DnugLeipzig.Extensions.DataObjects;
using DnugLeipzig.Extensions.Extensions;
using DnugLeipzig.Extensions.Filters;
using DnugLeipzig.Extensions.Repositories;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Macros
{
	[Chalk("events")]
	public class EventMacros : MacrosBase
	{
		readonly IEventConfigurationSource Configuration;

		#region ctors
		public EventMacros() : this(null, new EventPluginConfigurationSource())
		{
		}

		public EventMacros(IRepository<Post> repository, IEventConfigurationSource configuration) : base(configuration)
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
				return Configuration.UnknownText;
			}

			string dateFormat = Configuration.DateFormat;
			if (String.IsNullOrEmpty(dateFormat))
			{
				dateFormat = String.Format("{{0:{0}}}", SiteSettings.DateFormat);
			}

			return String.Format(dateFormat, post.Custom(Configuration.StartDateField).AsEventDate());
		}

		public string EndDate(Post post)
		{
			if (!post.Custom(Configuration.EndDateField).IsDate())
			{
				return Configuration.UnknownText;
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

			return String.Format(dateFormat, endDate);
		}

		public string Location(Post post)
		{
			string location = post.Custom(Configuration.LocationField);
			if (String.IsNullOrEmpty(location))
			{
				location = Configuration.UnknownText;
			}
			return location;
		}

		public override string Speaker(Post post)
		{
			return Speaker(post, Configuration.UnknownText, null, null);
		}
		#endregion

		public List<Post> GetForFuture()
		{
			return Repository.Get(new IsInFuture(Configuration.StartDateField),
								  new SortForIndexAscending(Configuration.StartDateField));
		}

		public List<Post> GetUpcoming(int numberOfEvents)
		{
			return Repository.Get(new HasDate(Configuration.StartDateField),
			                      new IsInFuture(Configuration.StartDateField),
			                      new SortForIndexAscending(Configuration.StartDateField),
			                      new LimitTo(numberOfEvents));
		}

		public List<Post> GetForYear(int year)
		{
			return Repository.Get(new IsInYear(Configuration.StartDateField, new DateTime(year, 1, 1)),
			                      new IsInPast(Configuration.StartDateField),
								  new SortForIndexAscending(Configuration.StartDateField));
		}

		public ICollection<PastPostInfo> GetPastYearOverview()
		{
			IList<Post> posts = Repository.Get(new IsInPast(Configuration.StartDateField));

			IEnumerable<PastPostInfo> pastEvents = from post in posts
			                                       group post by
			                                       	post.Custom(Configuration.StartDateField).AsEventDate().Year
			                                       into years orderby years.Key descending
			                                       	select
			                                       	new PastPostInfo
			                                       	{
			                                       		Year = years.Key,
			                                       		Url = Util.GetUrlForYearView(years.Key, Configuration.YearQueryString)
			                                       	};

			return new List<PastPostInfo>(pastEvents);
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
			       	Subject = post.Title,
			       	Description = SiteSettings.BaseUrl + post.Url,
			       	LastModified = post.Published,
			       	Categories = Repository.Data.Site.Title
			       };
		}
	}
}