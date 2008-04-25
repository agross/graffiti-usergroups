using System;
using System.Collections.Generic;
using System.Linq;

using DnugLeipzig.Definitions.Configuration;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Extensions.DataObjects;
using DnugLeipzig.Extensions.Extensions;
using DnugLeipzig.Runtime.Repositories;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Macros
{
	[Chalk("talks")]
	public class TalkMacros : Macros
	{
		readonly ITalkPluginConfiguration Configuration;

		#region ctors
		public TalkMacros() : this(null, new TalkPluginConfiguration())
		{
		}

		public TalkMacros(ICategoryEnabledRepository repository, ITalkPluginConfiguration configuration)
			: base(configuration)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}

			Configuration = configuration;

			if (repository == null)
			{
				repository = new TalkRepository(Configuration);
			}

			Repository = repository;
		}
		#endregion

		public IList<Post> GetForYear(int year)
		{
			return Repository.GetAll().IsInYear(Configuration.DateField, new DateTime(year, 1, 1)).
			                      SortAscending(Configuration.DateField).ToList();
		}

		public IList<Post> GetForCurrentYear()
		{
			return GetForYear(DateTime.Now.Year);
		}

		public IList<Post> GetRecent(int numberOfTalks)
		{
			return Repository.GetAll().HasDate(Configuration.DateField).
			                      IsInPast(Configuration.DateField).
								  SortDescending(Configuration.DateField).
			                      LimitTo(numberOfTalks).ToList();
		}

		public ICollection<PastPostInfo> GetPastYearOverview()
		{
			IEnumerable<Post> posts = Repository.GetAll().IsInPastYear(Configuration.DateField);

			IEnumerable<PastPostInfo> pastTalks = from post in posts
			                                      group post by
			                                      	post.Custom(Configuration.DateField).AsEventDate().Year
			                                      into years orderby years.Key descending
			                                      	select
			                                      	new PastPostInfo
			                                      	{
			                                      		Year = years.Key,
			                                      		Url = Util.GetUrlForYearView(years.Key, Configuration.YearQueryString)
			                                      	};

			return pastTalks.ToList();
		}
	}
}