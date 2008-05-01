using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DnugLeipzig.Definitions.Configuration;
using DnugLeipzig.Definitions.Extensions;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Extensions.DataObjects;
using DnugLeipzig.Extensions.Extensions;
using DnugLeipzig.Runtime.Repositories;

using Graffiti.Core;

using EventPluginConfiguration=DnugLeipzig.Extensions.Configuration.EventPluginConfiguration;

namespace DnugLeipzig.Extensions.Macros
{
	[Chalk("events")]
	public class EventMacros : Macros
	{
		readonly IEventPluginConfiguration _configuration;

		#region Ctors
		/// <summary>
		/// Initializes a new instance of the <see cref="EventMacros"/> class.
		/// </summary>
		public EventMacros() : this(null, new EventPluginConfiguration())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EventMacros"/> class.
		/// This constructor is used for dependency injection in unit testing scenarios.
		/// </summary>
		/// <param name="repository">The repository.</param>
		/// <param name="configuration">The configuration.</param>
		internal EventMacros(ICategoryEnabledRepository repository, IEventPluginConfiguration configuration)
			: base(configuration)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}

			_configuration = configuration;

			if (repository == null)
			{
				repository = new EventRepository(_configuration);
			}

			_repository = repository;
		}
		#endregion

		#region Dates, Location
		public string StartDate(Post post)
		{
			if (!post[_configuration.StartDateField].IsDate())
			{
				return HttpUtility.HtmlEncode(_configuration.UnknownText);
			}

			string dateFormat = _configuration.DateFormat;
			if (String.IsNullOrEmpty(dateFormat))
			{
				dateFormat = String.Format("{{0:{0}}}", SiteSettings.DateFormat);
			}

			return HttpUtility.HtmlEncode(String.Format(dateFormat, post[_configuration.StartDateField].AsEventDate()));
		}

		public string EndDate(Post post)
		{
			if (!post[_configuration.EndDateField].IsDate())
			{
				return HttpUtility.HtmlEncode(_configuration.UnknownText);
			}

			DateTime beginDate = post[_configuration.StartDateField].AsEventDate();
			DateTime endDate = post[_configuration.EndDateField].AsEventDate();

			string dateFormat = _configuration.DateFormat;
			if (beginDate.Date == endDate.Date && !String.IsNullOrEmpty(_configuration.ShortEndDateFormat))
			{
				dateFormat = _configuration.ShortEndDateFormat;
			}
			if (String.IsNullOrEmpty(dateFormat))
			{
				dateFormat = String.Format("{{0:{0}}}", SiteSettings.DateFormat);
			}

			return HttpUtility.HtmlEncode(String.Format(dateFormat, endDate));
		}

		public string Location(Post post)
		{
			string location = post[_configuration.LocationField];
			if (String.IsNullOrEmpty(location))
			{
				location = _configuration.UnknownText;
			}
			return HttpUtility.HtmlEncode(location);
		}

		public override string Speaker(Post post)
		{
			return Speaker(post, _configuration.UnknownText, null, null);
		}
		#endregion

		public IList<Post> GetForFuture()
		{
			return
				_repository.GetAll().IsInFuture(_configuration.StartDateField).SortAscending(_configuration.StartDateField).ToList();
		}

		public IList<Post> GetForRegistration()
		{
			return
				_repository.GetAll().HasDate(_configuration.StartDateField).IsInFuture(_configuration.StartDateField).
					RegistrationNeeded(_configuration.RegistrationNeededField).RegistrationPossible(
					_configuration.NumberOfRegistrationsField, _configuration.MaximumNumberOfRegistrationsField).SortAscending(
					_configuration.StartDateField).ToList();
		}

		public IList<Post> GetUpcoming(int numberOfEvents)
		{
			return
				_repository.GetAll().HasDate(_configuration.StartDateField).IsInFuture(_configuration.StartDateField).SortAscending(
					_configuration.StartDateField).LimitTo(numberOfEvents).ToList();
		}

		public IList<Post> GetForYear(int year)
		{
			return
				_repository.GetAll().IsInYear(_configuration.StartDateField, new DateTime(year, 1, 1)).IsInPast(
					_configuration.StartDateField).SortAscending(_configuration.StartDateField).ToList();
		}

		public ICollection<PastPostInfo> GetPastYearOverview()
		{
			IEnumerable<Post> posts = _repository.GetAll().IsInPast(_configuration.StartDateField);

			IEnumerable<PastPostInfo> pastEvents = from post in posts
			                                       group post by post[_configuration.StartDateField].AsEventDate().Year
			                                       into years orderby years.Key descending
			                                       	select
			                                       	new PastPostInfo
			                                       	{
			                                       		Year = years.Key,
			                                       		Url = Util.GetUrlForYearView(years.Key, _configuration.YearQueryString)
			                                       	};

			return pastEvents.ToList();
		}

		public bool RegistrationPossible(Post post)
		{
			return post.HasDate(_configuration.StartDateField) && post.IsInFuture(_configuration.StartDateField) &&
			       post.RegistrationNeeded(_configuration.RegistrationNeededField) &&
			       post.RegistrationPossible(_configuration.NumberOfRegistrationsField,
			                                 _configuration.MaximumNumberOfRegistrationsField);
		}

		public string RegisterButton(IDictionary properties)
		{
			string scriptPath =
				VirtualPathUtility.ToAbsolute(String.Format("~/files/themes/{0}/handlers/Register.ashx",
				                                            GraffitiContext.Current.Theme));

			string cssClass = properties.GetAsAttribute("class");
			string text = properties.GetAsAttribute("value");
			string id = properties.GetAsAttribute("id");

			return string.Format("<input {0} {1} {2} type=\"button\" onclick=\"Register.submitMessage('{3}');\" />",
			                     id,
			                     cssClass,
			                     text,
			                     scriptPath);
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
			       	StartDate = post[_configuration.StartDateField].AsEventDate(),
			       	EndDate = post[_configuration.EndDateField].AsEventDate(),
			       	Location = post[_configuration.LocationField],
			       	Subject = HttpUtility.HtmlDecode(post.Title),
			       	Description = SiteSettings.BaseUrl + post.Url,
			       	LastModified = post.Published,
			       	Categories = HttpUtility.HtmlDecode(_repository.GraffitiData.Site.Title)
			       };
		}
	}
}