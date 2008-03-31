using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using DnugLeipzig.Extensions.DataObjects;
using DnugLeipzig.Extensions.Extensions;
using DnugLeipzig.Extensions.Filters;
using DnugLeipzig.Extensions.Repositories;

using Graffiti.Core;

namespace DnugLeipzig.Extensions
{
	[Chalk("ugTalks")]
	public class UserGroupTalks
	{
		static readonly string CategoryName;
		static readonly string DateFieldName;
		static readonly Macros Macros = new Macros();
		static readonly string SpeakerFieldName;
		static readonly string YearQueryStringParameter;
		readonly IRepository<Post> Repository;

		static UserGroupTalks()
		{
			CategoryName = ConfigurationManager.AppSettings.GetOrDefault("UserGroup:Talks:CategoryName", "Vortr&#228;ge");
			DateFieldName = ConfigurationManager.AppSettings.GetOrDefault("UserGroup:Talks:DateFieldName", "Datum");
			SpeakerFieldName = ConfigurationManager.AppSettings.GetOrDefault("UserGroup:Talks:SpeakerFieldName", "Sprecher");
			YearQueryStringParameter =
				ConfigurationManager.AppSettings.GetOrDefault("UserGroup:Talks:YearQueryStringParameterName", "year");
		}

		public UserGroupTalks() : this(null)
		{
		}

		public UserGroupTalks(IRepository<Post> repository)
		{
			if (repository == null)
			{
				repository = new TalkRepository(CategoryName, DateFieldName);
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

		public List<Post> GetForYear(int year)
		{
			return Repository.Get(new IsInYear(DateFieldName, new DateTime(year, 1, 1)), new SortForIndexDescending(DateFieldName));
		}

		public List<Post> GetForCurrentYear()
		{
			return GetForYear(DateTime.Now.Year);
		}

		public List<Post> GetRecent(int numberOfTalks)
		{
			return Repository.Get(new HasDate(DateFieldName),
								  new IsInPast(DateFieldName),
								  new SortForIndexDescending(DateFieldName),
								  new LimitTo(numberOfTalks));
		}

		public ICollection<PastPostInfo> GetPastYearOverview()
		{
			IList<Post> posts = Repository.Get(new IsInPastYear(DateFieldName));

			IEnumerable<PastPostInfo> pastTalks = from post in posts
			                                      group post by post.Custom(DateFieldName).AsEventDate().Year
			                                      into years orderby years.Key descending
			                                      	select
			                                      	new PastPostInfo
			                                      	{
			                                      		Year = years.Key,
			                                      		Url = Util.GetUrlForYearView(years.Key, YearQueryStringParameter)
			                                      	};

			return new List<PastPostInfo>(pastTalks);
		}

		public int? GetViewYear()
		{
			return Util.GetViewYear(YearQueryStringParameter);
		}
	}
}