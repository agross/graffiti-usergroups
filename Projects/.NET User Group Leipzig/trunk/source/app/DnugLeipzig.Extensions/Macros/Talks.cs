using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using DnugLeipzig.Extensions.DataObjects;
using DnugLeipzig.Extensions.Extensions;
using DnugLeipzig.Extensions.Filters;
using DnugLeipzig.Extensions.Repositories;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Macros
{
	// TODO: Refactor into a plugin and let the user define the values of the various fields using the Graffiti UI.
	[Chalk("talks")]
	public class Talks : MacroBase
	{
		#region ctors
		public Talks() : this(null)
		{
		}

		public Talks(IRepository<Post> repository)
		{
			CategoryName = ConfigurationManager.AppSettings.GetOrDefault("Talks:CategoryName", "Vortr&#228;ge");
			SortRelevantDate = ConfigurationManager.AppSettings.GetOrDefault("Talks:DateField", "Datum");
			SpeakerField = ConfigurationManager.AppSettings.GetOrDefault("Talks:SpeakerField", "Sprecher");
			YearQueryStringParameter =
				ConfigurationManager.AppSettings.GetOrDefault("Talks:YearQueryStringParameterName", "year");

            if (repository == null)
            {
                repository = new TalkRepository(CategoryName, SortRelevantDate);
            }

            Repository = repository;
		}
		#endregion

		#region Retrieval
		public List<Post> GetForYear(int year)
		{
			return Repository.Get(new IsInYear(SortRelevantDate, new DateTime(year, 1, 1)),
			                      new SortForIndexDescending(SortRelevantDate));
		}

		public List<Post> GetForCurrentYear()
		{
			return GetForYear(DateTime.Now.Year);
		}

		public List<Post> GetRecent(int numberOfTalks)
		{
			return Repository.Get(new HasDate(SortRelevantDate),
			                      new IsInPast(SortRelevantDate),
			                      new SortForIndexDescending(SortRelevantDate),
			                      new LimitTo(numberOfTalks));
		}

		public ICollection<PastPostInfo> GetPastYearOverview()
		{
			IList<Post> posts = Repository.Get(new IsInPastYear(SortRelevantDate));

			IEnumerable<PastPostInfo> pastTalks = from post in posts
			                                      group post by
			                                      	post.Custom(SortRelevantDate).AsEventDate().Year
			                                      into years orderby years.Key descending
			                                      	select
			                                      	new PastPostInfo
			                                      	{
			                                      		Year = years.Key,
			                                      		Url = Util.GetUrlForYearView(years.Key, YearQueryStringParameter)
			                                      	};

			return new List<PastPostInfo>(pastTalks);
		}
		#endregion
	}
}