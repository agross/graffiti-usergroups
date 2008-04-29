using System;
using System.Collections.Generic;
using System.Linq;

using DnugLeipzig.Definitions.Configuration;
using DnugLeipzig.Definitions.Extensions;
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
		readonly ITalkPluginConfiguration _configuration;

		#region Ctors
		/// <summary>
		/// Initializes a new instance of the <see cref="TalkMacros"/> class.
		/// </summary>
		public TalkMacros() : this(null, new TalkPluginConfiguration())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TalkMacros"/> class.
		/// This constructor is used for dependency injection in unit testing scenarios.
		/// </summary>
		/// <param name="repository">The repository.</param>
		/// <param name="configuration">The configuration.</param>
		internal TalkMacros(ICategoryEnabledRepository repository, ITalkPluginConfiguration configuration)
			: base(configuration)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}

			_configuration = configuration;

			if (repository == null)
			{
				repository = new TalkRepository(_configuration);
			}

			_repository = repository;
		}
		#endregion

		public IList<Post> GetForYear(int year)
		{
			return
				_repository.GetAll().IsInYear(_configuration.DateField, new DateTime(year, 1, 1)).SortAscending(
					_configuration.DateField).ToList();
		}

		public IList<Post> GetForCurrentYear()
		{
			return GetForYear(DateTime.Now.Year);
		}

		public IList<Post> GetRecent(int numberOfTalks)
		{
			return
				_repository.GetAll().HasDate(_configuration.DateField).IsInPast(_configuration.DateField).SortDescending(
					_configuration.DateField).LimitTo(numberOfTalks).ToList();
		}

		public ICollection<PastPostInfo> GetPastYearOverview()
		{
			IEnumerable<Post> posts = _repository.GetAll().IsInPastYear(_configuration.DateField);

			IEnumerable<PastPostInfo> pastTalks = from post in posts
			                                      group post by post[_configuration.DateField].AsEventDate().Year
			                                      into years orderby years.Key descending
			                                      	select
			                                      	new PastPostInfo
			                                      	{
			                                      		Year = years.Key,
			                                      		Url = Util.GetUrlForYearView(years.Key, _configuration.YearQueryString)
			                                      	};

			return pastTalks.ToList();
		}
	}
}