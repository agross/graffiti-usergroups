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
		static readonly string DateFieldName;
		static readonly string DateFormat;
		static readonly string DefaultLocationText;
		static readonly string LocationFieldName;
		static readonly Macros Macros = new Macros();
		static readonly string SpeakerFieldName;
		static readonly string UnknownText;
		static readonly string YearQueryStringParameter;
		readonly IRepository<Post> Repository;

		static UserGroupEvents()
		{
			DefaultLocationText = ConfigurationManager.AppSettings.GetOrDefault("UserGroup:Events:DefaultLocationText",
			                                                                    "Marschnerstraﬂe");
			CategoryName = ConfigurationManager.AppSettings.GetOrDefault("UserGroup:Events:CategoryName", "Veranstaltungen");
			DateFieldName = ConfigurationManager.AppSettings.GetOrDefault("UserGroup:Events:DateFieldName", "Datum");
			LocationFieldName = ConfigurationManager.AppSettings.GetOrDefault("UserGroup:Events:LocationFieldName", "Ort");
			SpeakerFieldName = ConfigurationManager.AppSettings.GetOrDefault("UserGroup:Events:SpeakerFieldName", "Sprecher");
			UnknownText = ConfigurationManager.AppSettings.GetOrDefault("UserGroup:Events:UnknownText", "Noch nicht bekannt");
			DateFormat = ConfigurationManager.AppSettings.GetOrDefault("UserGroup:Events:DateFormat", "{0:D}, Beginn: {0:t} Uhr");
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

		public string Date(Post post, string prefix, string suffix)
		{
			DateTime date;
			if (!DateTime.TryParse(post.Custom(DateFieldName), out date))
			{
				return String.Empty;
			}

			return HttpUtility.HtmlEncode(String.Format("{0}{1}{2}", prefix, Macros.FormattedDate(date), suffix));
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
			List<Post> posts = Repository.Get(new IsInFutureFilter(DateFieldName));
			posts.SortForIndexView();
			return posts;
		}

		public List<Post> GetForYear(int year)
		{
			List<Post> posts = Repository.Get(new IsInYearFilter(DateFieldName, new DateTime(year, 1, 1)), new IsInPastFilter(DateFieldName));
			posts.SortForIndexView();
			return posts;
		}

		public ICollection<PastPostInfo> GetPastYearOverview()
		{
			IList<Post> posts = Repository.Get(new IsInPastFilter(DateFieldName));

			IEnumerable<PastPostInfo> pastEvents = from post in posts
			                                       group post by post.Custom(DateFieldName).AsEventDate().Year
			                                       into years orderby years.Key descending
			                                       	select
			                                       	new PastPostInfo
			                                       	{
			                                       		Year = years.Key,
			                                       		Url = Util.GetUrlForYearView(years.Key, YearQueryStringParameter)
			                                       	};

			return new List<PastPostInfo>(pastEvents);
		}

		public IDictionary<string, string> GetEventInfo(Post post)
		{
			var result = new Dictionary<string, string>();

			// Only add explicit properties.
			result.Add(DateFieldName, GetEventDate(post));
			result.Add(SpeakerFieldName, GetSpeaker(post));
			result.Add(LocationFieldName, GetLocation(post));

			return result;
		}

		static string GetLocation(Post post)
		{
			string location = post.Custom(LocationFieldName);
			if (String.IsNullOrEmpty(location))
			{
				location = DefaultLocationText;
			}
			return location;
		}

		static string GetSpeaker(Post post)
		{
			string speaker = post.Custom(SpeakerFieldName);
			if (String.IsNullOrEmpty(speaker))
			{
				speaker = UnknownText;
			}
			return speaker;
		}

		static string GetEventDate(Post post)
		{
			string eventDate;
			if (post.Custom(DateFieldName).IsDate())
			{
				eventDate = String.Format(DateFormat, post.Custom(DateFieldName).AsEventDate());
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
	}
}