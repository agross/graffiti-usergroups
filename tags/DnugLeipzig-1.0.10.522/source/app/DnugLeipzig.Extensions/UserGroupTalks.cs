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

		public List<Post> GetForYear(int year)
		{
			List<Post> posts = Repository.Get(new IsInYearFilter(DateFieldName, new DateTime(year, 1, 1)));
			posts.SortForIndexView();

			return posts;
		}

		public List<Post> GetCurrent()
		{
			List<Post> posts = Repository.Get(new IsInYearFilter(DateFieldName, DateTime.Now));
			posts.SortForIndexView();

			return posts;
		}

		public ICollection<PastPostInfo> GetPastYearOverview()
		{
			IList<Post> posts = Repository.Get(new IsInPastYearFilter(DateFieldName));

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