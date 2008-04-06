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
	[Chalk("talks")]
	public class TalkMacros : MacrosBase
	{
		readonly ITalkConfigurationSource Configuration;

		#region ctors
		public TalkMacros() : this(null, new TalkPluginConfigurationSource())
		{
		}

		public TalkMacros(IRepository<Post> repository, ITalkConfigurationSource configuration) : base(configuration)
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

		public List<Post> GetForYear(int year)
		{
			return Repository.Get(new IsInYear(Configuration.DateField, new DateTime(year, 1, 1)),
			                      new SortForIndexAscending(Configuration.DateField));
		}

		public List<Post> GetForCurrentYear()
		{
			return GetForYear(DateTime.Now.Year);
		}

		public List<Post> GetRecent(int numberOfTalks)
		{
			return Repository.Get(new HasDate(Configuration.DateField),
			                      new IsInPast(Configuration.DateField),
								  new SortForIndexAscending(Configuration.DateField),
			                      new LimitTo(numberOfTalks));
		}

		public ICollection<PastPostInfo> GetPastYearOverview()
		{
			IList<Post> posts = Repository.Get(new IsInPastYear(Configuration.DateField));

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

			return new List<PastPostInfo>(pastTalks);
		}
	}
}