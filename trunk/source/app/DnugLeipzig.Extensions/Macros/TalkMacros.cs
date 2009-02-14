using System;
using System.Collections.Generic;
using System.Linq;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Extensions;
using DnugLeipzig.Definitions.Plugins.Talks;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Extensions.DataObjects;
using DnugLeipzig.Extensions.Extensions;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Macros
{
	[Chalk("talks")]
	public class TalkMacros : Macros<ITalkPluginConfigurationProvider>
	{
		#region Ctors
		/// <summary>
		/// Initializes a new instance of the <see cref="TalkMacros"/> class.
		/// </summary>
		public TalkMacros() : this(IoC.Resolve<ICategorizedPostRepository<ITalkPluginConfigurationProvider>>())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EventMacros"/> class.
		/// This constructor is used for dependency injection in unit testing scenarios.
		/// </summary>
		/// <param name="repository">The repository.</param>
		internal TalkMacros(ICategorizedPostRepository<ITalkPluginConfigurationProvider> repository)
			: base(repository)
		{
		}
		#endregion

		public IList<Post> GetForYear(int year)
		{
			return
				Repository.GetAll()
					.IsInYear(Configuration.DateField, new DateTime(year, 1, 1))
					.SortAscending(Configuration.DateField)
					.ToList();
		}

		public IList<Post> GetForCurrentYear()
		{
			return GetForYear(DateTime.Now.Year);
		}

		public IList<Post> GetRecent(int numberOfTalks)
		{
			return
				Repository.GetAll()
					.HasDate(Configuration.DateField)
					.IsInPast(Configuration.DateField)
					.SortDescending(Configuration.DateField)
					.LimitTo(numberOfTalks)
					.ToList();
		}

		public ICollection<PastPostInfo> GetPastYearOverview()
		{
			IEnumerable<Post> posts = Repository.GetAll().IsInPastYear(Configuration.DateField);

			IEnumerable<PastPostInfo> pastTalks = from post in posts
			                                      group post by post[Configuration.DateField].AsEventDate().Year
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