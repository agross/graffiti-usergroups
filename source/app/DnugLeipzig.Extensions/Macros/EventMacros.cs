using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Configuration.Plugins;
using DnugLeipzig.Definitions.Extensions;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Extensions.DataObjects;
using DnugLeipzig.Extensions.Extensions;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Macros
{
	[Chalk("events")]
	public class EventMacros : Macros<IEventPluginConfiguration>
	{
		#region Ctors
		/// <summary>
		/// Initializes a new instance of the <see cref="EventMacros"/> class.
		/// </summary>
		public EventMacros() : this(IoC.Resolve<ICategorizedPostRepository<IEventPluginConfiguration>>())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EventMacros"/> class.
		/// This constructor is used for dependency injection in unit testing scenarios.
		/// </summary>
		/// <param name="repository">The repository.</param>
		internal EventMacros(ICategorizedPostRepository<IEventPluginConfiguration> repository)
			: base(repository)
		{
		}
		#endregion

		#region Dates, Location
		public string StartDate(Post post)
		{
			if (!post[Configuration.StartDateField].IsDate())
			{
				return HttpUtility.HtmlEncode(Configuration.UnknownText);
			}

			string dateFormat = Configuration.DateFormat;
			if (String.IsNullOrEmpty(dateFormat))
			{
				dateFormat = String.Format("{{0:{0}}}", SiteSettings.DateFormat);
			}

			return HttpUtility.HtmlEncode(String.Format(dateFormat, post[Configuration.StartDateField].AsEventDate()));
		}

		public string EndDate(Post post)
		{
			if (!post[Configuration.EndDateField].IsDate())
			{
				return HttpUtility.HtmlEncode(Configuration.UnknownText);
			}

			DateTime beginDate = post[Configuration.StartDateField].AsEventDate();
			DateTime endDate = post[Configuration.EndDateField].AsEventDate();

			string dateFormat = Configuration.DateFormat;
			if (beginDate.Date == endDate.Date && !String.IsNullOrEmpty(Configuration.ShortEndDateFormat))
			{
				dateFormat = Configuration.ShortEndDateFormat;
			}
			if (String.IsNullOrEmpty(dateFormat))
			{
				dateFormat = String.Format("{{0:{0}}}", SiteSettings.DateFormat);
			}

			return HttpUtility.HtmlEncode(String.Format(dateFormat, endDate));
		}

		public string Location(Post post)
		{
			string location = post[Configuration.LocationField];
			if (String.IsNullOrEmpty(location))
			{
				location = Configuration.UnknownText;
			}
			return HttpUtility.HtmlEncode(location);
		}

		public override string Speaker(Post post)
		{
			return Speaker(post, Configuration.UnknownText, null, null);
		}
		#endregion

		public IList<Post> GetForFuture()
		{
			return
				Repository.GetAll()
					.IsInFuture(Configuration.StartDateField)
					.SortAscending(Configuration.StartDateField)
					.ToList();
		}

		public IList<Post> GetForRegistration()
		{
			return
				Repository.GetAll()
					.HasDate(Configuration.StartDateField)
					.IsInFuture(Configuration.StartDateField)
					.RegistrationNeeded(Configuration.RegistrationNeededField)
					.RegistrationPossible(Configuration.RegistrationListField, Configuration.MaximumNumberOfRegistrationsField)
					.SortAscending(Configuration.StartDateField).ToList();
		}

		public IList<Post> GetUpcoming(int numberOfEvents)
		{
			return
				Repository.GetAll()
					.HasDate(Configuration.StartDateField)
					.IsInFuture(Configuration.StartDateField)
					.SortAscending(Configuration.StartDateField)
					.LimitTo(numberOfEvents)
					.ToList();
		}

		public IList<Post> GetForYear(int year)
		{
			return
				Repository.GetAll()
					.IsInYear(Configuration.StartDateField, new DateTime(year, 1, 1))
					.IsInPast(Configuration.StartDateField)
					.SortAscending(Configuration.StartDateField)
					.ToList();
		}

		public ICollection<PastPostInfo> GetPastYearOverview()
		{
			IEnumerable<Post> posts = Repository.GetAll().IsInPast(Configuration.StartDateField);

			IEnumerable<PastPostInfo> pastEvents = from post in posts
			                                       group post by post[Configuration.StartDateField].AsEventDate().Year
			                                       into years orderby years.Key descending
			                                       	select
			                                       	new PastPostInfo
			                                       	{
			                                       		Year = years.Key,
			                                       		Url = Util.GetUrlForYearView(years.Key, Configuration.YearQueryString)
			                                       	};

			return pastEvents.ToList();
		}

		public bool RegistrationPossible(Post post)
		{
			return post.HasDate(Configuration.StartDateField) && post.IsInFuture(Configuration.StartDateField) &&
			       post.RegistrationNeeded(Configuration.RegistrationNeededField) &&
			       post.RegistrationPossible(Configuration.RegistrationListField,
			                                 Configuration.MaximumNumberOfRegistrationsField);
		}

		public string RegisterButton(IDictionary properties, bool generateOnClickHandler)
		{
			string cssClass = properties.GetAsAttribute("class");
			string text = properties.GetAsAttribute("value");
			string id = properties.GetAsAttribute("id");

			string onClickHandler = null;
			if (generateOnClickHandler)
			{
				onClickHandler = String.Format("onclick=\"Register.submitMessage('{0}');\"", RegistrationHandler());
			}

			return string.Format("<input {0} {1} {2} type=\"submit\" {3}/>", id, cssClass, text, onClickHandler);
		}

		public string RegistrationHandler()
		{
			return
				VirtualPathUtility.ToAbsolute(String.Format("~/files/themes/{0}/handlers/Register.ashx",
				                                            GraffitiContext.Current.Theme));
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
			       	StartDate = post[Configuration.StartDateField].AsEventDate(),
			       	EndDate = post[Configuration.EndDateField].AsEventDate(),
			       	Location = post[Configuration.LocationField],
			       	Subject = HttpUtility.HtmlDecode(post.Title),
			       	Description = SiteSettings.BaseUrl + post.Url,
			       	LastModified = post.Published,
			       	Categories = HttpUtility.HtmlDecode(Repository.GraffitiData.Site.Title)
			       };
		}
	}
}