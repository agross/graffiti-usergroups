using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using DnugLeipzig.Extensions.DataObjects;
using DnugLeipzig.Extensions.Extensions;
using DnugLeipzig.Extensions.Filters;
using DnugLeipzig.Extensions.Macros;
using DnugLeipzig.Extensions.Repositories;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Macros
{
	// TODO: Refactor into a plugin and let the user define the values of the various fields using the Graffiti UI.
	[Chalk("events")]
	public class Events : MacroBase
	{
		readonly string StartDateField;
		readonly string DateFormat;
		readonly string ShortDateFormat;
		readonly string LocationField;
		readonly string UnknownText;
		readonly string EndDateField;

		#region ctors
		public Events() : this(null)
		{
		}

		public Events(IRepository<Post> repository)
		{
			CategoryName = ConfigurationManager.AppSettings.GetOrDefault("Events:CategoryName", "Veranstaltungen");
			SortRelevantDate = StartDateField = ConfigurationManager.AppSettings.GetOrDefault("Events:StartDateField", "Datum Anfang");
			EndDateField = ConfigurationManager.AppSettings.GetOrDefault("Events:EndDateField", "Datum Ende");
			LocationField = ConfigurationManager.AppSettings.GetOrDefault("Events:LocationField", "Ort");
			SpeakerField = ConfigurationManager.AppSettings.GetOrDefault("Events:SpeakerField", "Sprecher");
			UnknownText = ConfigurationManager.AppSettings.GetOrDefault("Events:UnknownText", "Noch nicht bekannt");
			DateFormat = ConfigurationManager.AppSettings.GetOrDefault("Events:DateFormat", "{0:D}, {0:t} Uhr");
			ShortDateFormat = ConfigurationManager.AppSettings.GetOrDefault("Events:ShortDateFormat", "{0:t} Uhr");
			YearQueryStringParameter =
				ConfigurationManager.AppSettings.GetOrDefault("UserGroup:Events:YearQueryStringParameterName", "year");

			if (repository == null)
			{
				repository = new EventRepository(CategoryName);
			}

			Repository = repository;
		}
		#endregion

		#region Dates, Location
		public string StartDate(Post post)
		{
			string eventDate;
			if (post.Custom(StartDateField).IsDate())
			{
				eventDate = String.Format(DateFormat, post.Custom(StartDateField).AsEventDate());
			}
			else
			{
				eventDate = UnknownText;
			}
			return eventDate;
		}

		public string EndDate(Post post)
		{
			string eventDate;
			if (post.Custom(EndDateField).IsDate())
			{
				DateTime beginDate = post.Custom(StartDateField).AsEventDate();
				DateTime endDate = post.Custom(EndDateField).AsEventDate();

				if (beginDate.Date == endDate.Date)
				{
					eventDate = String.Format(ShortDateFormat, endDate);
				}
				else
				{
					eventDate = String.Format(DateFormat, endDate);
				}
			}
			else
			{
				eventDate = UnknownText;
			}
			return eventDate;
		}

		public string Location(Post post)
		{
			string location = post.Custom(LocationField);
			if (String.IsNullOrEmpty(location))
			{
				location = UnknownText;
			}
			return location;
		}

		public new string Speaker(Post post)
		{
			return Speaker(post, UnknownText, null, null);
		}
		#endregion

		public List<Post> GetForFuture()
		{
			return Repository.Get(new IsInFuture(StartDateField), new SortForIndexDescending(StartDateField));
		}

		public List<Post> GetUpcoming(int numberOfEvents)
		{
			return Repository.Get(new HasDate(StartDateField),
			                      new IsInFuture(StartDateField),
			                      new SortForIndexAscending(StartDateField),
			                      new LimitTo(numberOfEvents));
		}

		public List<Post> GetForYear(int year)
		{
			return Repository.Get(new IsInYear(StartDateField, new DateTime(year, 1, 1)),
			                      new IsInPast(StartDateField),
			                      new SortForIndexDescending(StartDateField));
		}

		public ICollection<PastPostInfo> GetPastYearOverview()
		{
			IList<Post> posts = Repository.Get(new IsInPast(StartDateField));

			IEnumerable<PastPostInfo> pastEvents = from post in posts
			                                       group post by post.Custom(StartDateField).AsEventDate().Year
			                                       into years orderby years.Key descending
			                                       	select
			                                       	new PastPostInfo
			                                       	{
			                                       		Year = years.Key,
			                                       		Url = Util.GetUrlForYearView(years.Key, YearQueryStringParameter)
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
			       	StartDate = post.Custom(StartDateField).AsEventDate(),
			       	EndDate = post.Custom(EndDateField).AsEventDate(),
			       	Location = post.Custom(LocationField),
			       	Subject = post.Title,
			       	Description = SiteSettings.BaseUrl + post.Url,
			       	LastModified = post.Published,
			       	Categories = Repository.Data.Site.Title
			       };
		}
	}
}