using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

using DnugLeipzig.Extensions.DataObjects;
using DnugLeipzig.Extensions.Extensions;
using DnugLeipzig.Extensions.Filters;
using DnugLeipzig.Extensions.Repositories;

using Graffiti.Core;

namespace DnugLeipzig.Extensions
{
	[Chalk("ugEvents")]
	public class UserGroupEvents
	{
		static readonly string CategoryName;
		static readonly string BeginDateFieldName;
		static readonly string DateFormat;
		static readonly string ShortDateFormat;
		static readonly string DefaultLocationText;
		static readonly string LocationFieldName;
		static readonly Macros Macros = new Macros();
		static readonly string SpeakerFieldName;
		static readonly string UnknownText;
		static readonly string YearQueryStringParameter;
		readonly IRepository<Post> Repository;
		static string EndDateFieldName;

		static UserGroupEvents()
		{
			DefaultLocationText = ConfigurationManager.AppSettings.GetOrDefault("UserGroup:Events:DefaultLocationText",
			                                                                    "Marschnerstraﬂe");
			CategoryName = ConfigurationManager.AppSettings.GetOrDefault("UserGroup:Events:CategoryName", "Veranstaltungen");
			BeginDateFieldName = ConfigurationManager.AppSettings.GetOrDefault("UserGroup:Events:BeginDateFieldName", "Datum Anfang");
			EndDateFieldName = ConfigurationManager.AppSettings.GetOrDefault("UserGroup:Events:EndDateFieldName", "Datum Ende");
			LocationFieldName = ConfigurationManager.AppSettings.GetOrDefault("UserGroup:Events:LocationFieldName", "Ort");
			SpeakerFieldName = ConfigurationManager.AppSettings.GetOrDefault("UserGroup:Events:SpeakerFieldName", "Sprecher");
			UnknownText = ConfigurationManager.AppSettings.GetOrDefault("UserGroup:Events:UnknownText", "Noch nicht bekannt");
			DateFormat = ConfigurationManager.AppSettings.GetOrDefault("UserGroup:Events:DateFormat", "{0:D}, {0:t} Uhr");
			ShortDateFormat = ConfigurationManager.AppSettings.GetOrDefault("UserGroup:Events:ShortDateFormat", "{0:t} Uhr");
			YearQueryStringParameter =
				ConfigurationManager.AppSettings.GetOrDefault("UserGroup:Events:YearQueryStringParameterName", "year");
		}

		public UserGroupEvents() : this(null)
		{
		}

		public UserGroupEvents(IRepository<Post> repository)
		{
			if (repository == null)
			{
				repository = new EventRepository(CategoryName);
			}

			Repository = repository;
		}

		public string GetFeedUrl()
		{
			Category c = Repository.GetCategory();
			if (!String.IsNullOrEmpty(c.FeedUrlOverride))
			{
				return c.FeedUrlOverride;
			}

			return String.Format("{0}feed/", c.Url);
		}

		public string GetCategoryLink()
		{
			Category c = Repository.GetCategory();
			return c.Url;
		}

		public string Date(Post post, string prefix, string suffix)
		{
			DateTime date;
			if (!DateTime.TryParse(post.Custom(BeginDateFieldName), out date))
			{
				return String.Empty;
			}

			return HttpUtility.HtmlEncode(String.Format("{0}{1}{2}", prefix, Macros.FormattedDate(date), suffix));
		}

		public string Date(Post post, string format)
		{
			DateTime date;
			if (!DateTime.TryParse(post.Custom(BeginDateFieldName), out date))
			{
				return String.Empty;
			}

			return HttpUtility.HtmlEncode(date.ToString(format));
		}

		public string Speaker(Post post, string prefix, string suffix)
		{
			string speaker = post.Custom(SpeakerFieldName);
			if (speaker == null || speaker.Trim().Length == 0)
			{
				return String.Empty;
			}

			return HttpUtility.HtmlEncode(String.Format("{0}{1}{2}", prefix, speaker, suffix));
		}

		public string Title(Post post, string datePrefix, string speakerPrefix)
		{
			return String.Format("{0}{1}{2}",
			                     post.Title,
			                     Date(post, datePrefix, String.Empty),
			                     Speaker(post, speakerPrefix, String.Empty));
		}

		public List<Post> GetForFuture()
		{
			return Repository.Get(new IsInFuture(BeginDateFieldName), new SortForIndexDescending(BeginDateFieldName));
		}

		public List<Post> GetUpcoming(int numberOfEvents)
		{
			return Repository.Get(new HasDate(BeginDateFieldName),
			                      new IsInFuture(BeginDateFieldName),
			                      new SortForIndexAscending(BeginDateFieldName),
			                      new LimitTo(numberOfEvents));
		}

		public List<Post> GetForYear(int year)
		{
			return Repository.Get(new IsInYear(BeginDateFieldName, new DateTime(year, 1, 1)),
			                      new IsInPast(BeginDateFieldName),
			                      new SortForIndexDescending(BeginDateFieldName));
		}

		public ICollection<PastPostInfo> GetPastYearOverview()
		{
			IList<Post> posts = Repository.Get(new IsInPast(BeginDateFieldName));

			IEnumerable<PastPostInfo> pastEvents = from post in posts
			                                       group post by post.Custom(BeginDateFieldName).AsEventDate().Year
			                                       into years orderby years.Key descending
			                                       	select
			                                       	new PastPostInfo
			                                       	{
			                                       		Year = years.Key,
			                                       		Url = Util.GetUrlForYearView(years.Key, YearQueryStringParameter)
			                                       	};

			return new List<PastPostInfo>(pastEvents);
		}

		public string GetLocation(Post post)
		{
			string location = post.Custom(LocationFieldName);
			if (String.IsNullOrEmpty(location))
			{
				location = DefaultLocationText;
			}
			return location;
		}

		public string GetSpeaker(Post post)
		{
			string speaker = post.Custom(SpeakerFieldName);
			if (String.IsNullOrEmpty(speaker))
			{
				speaker = UnknownText;
			}
			return speaker;
		}

		public bool IsInCurrentYear(Post post)
		{
			return post.Custom(BeginDateFieldName).AsEventDate().Year == DateTime.Now.Year;
		}

		public bool HasDate(Post post)
		{
			return post.Custom(BeginDateFieldName).IsDate();
		}

		public string GetBeginDate(Post post)
		{
			string eventDate;
			if (post.Custom(BeginDateFieldName).IsDate())
			{
				eventDate = String.Format(DateFormat, post.Custom(BeginDateFieldName).AsEventDate());
			}
			else
			{
				eventDate = UnknownText;
			}
			return eventDate;
		}

		public string GetEndDate(Post post)
		{
			string eventDate;
			if (post.Custom(EndDateFieldName).IsDate())
			{
				DateTime beginDate = post.Custom(BeginDateFieldName).AsEventDate();
				DateTime endDate = post.Custom(EndDateFieldName).AsEventDate();

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

		public int? GetViewYear()
		{
			return Util.GetViewYear(YearQueryStringParameter);
		}

		public bool CanCreateCalendarItem(Post post)
		{
			CalendarItem item = CreateCalendarItem(post);
			return item.IsValid();
		}

		public CalendarItem CreateCalendarItem(Post post)
		{
			CalendarItem item = new CalendarItem
			                    {
			                    	StartDate = post.Custom(BeginDateFieldName).AsEventDate(),
			                    	EndDate = post.Custom(EndDateFieldName).AsEventDate(),
			                    	Location = post.Custom(LocationFieldName),
			                    	Subject = post.Title,
									Description = SiteSettings.BaseUrl + post.Url,
			                    	LastModified = post.Published,
			                    	Categories = Repository.Data.Site.Title
			                    };

			return item;
		}
	}
}