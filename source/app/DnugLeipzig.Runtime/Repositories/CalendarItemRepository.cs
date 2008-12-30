using System;
using System.Web;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Configuration;
using DnugLeipzig.Definitions.Configuration.Plugins;
using DnugLeipzig.Definitions.Extensions;
using DnugLeipzig.Definitions.Repositories;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Repositories
{
	public class CalendarItemRepository : Repository, ICalendarItemRepository
	{
		readonly IEventPluginConfiguration _eventPluginConfiguration;
		readonly IPostRepository _postRepository;
		readonly IGraffitiSiteSettings _settings;

		public CalendarItemRepository(IPostRepository postRepository,
		                              IEventPluginConfiguration eventPluginConfiguration,
		                              IGraffitiSiteSettings settings)
		{
			_postRepository = postRepository;
			_eventPluginConfiguration = eventPluginConfiguration;
			_settings = settings;
		}

		#region Implementation of ICalendarItemRepository
		public ICalendarItem GetCalendarItemForEvent(int eventId)
		{
			try
			{
				Post post = _postRepository.GetById(eventId);
				if (post == null)
				{
					return null;
				}

				return CreateCalendarItemFrom(post);
			}
			catch (Exception ex)
			{
				Logger.Error(Create.New.Message().WithTitle("Could not create calendar item from post {0}", eventId), ex);
				return null;
			}
		}
		#endregion

		CalendarItem CreateCalendarItemFrom(Post post)
		{
			string postUrl = _settings.BaseUrl;

			try
			{
				postUrl += post.Url;
			}
				// ReSharper disable EmptyGeneralCatchClause
			catch
				// ReSharper restore EmptyGeneralCatchClause
			{
				// In unit-testing scenarios, accessing URL causes database access. Well done, telligent.
			}

			var calendarItem = new CalendarItem
			                   {
			                   	StartDate = post[_eventPluginConfiguration.StartDateField].AsEventDate(),
			                   	EndDate = post[_eventPluginConfiguration.EndDateField].AsEventDate(),
			                   	Location = post[_eventPluginConfiguration.LocationUnknownField].IsChecked()
			                   	           	? _eventPluginConfiguration.UnknownText
			                   	           	: post[_eventPluginConfiguration.LocationField],
			                   	Subject = HttpUtility.HtmlDecode(post.Title),
			                   	Description = postUrl,
			                   	LastModified = post.Published,
			                   	Categories = HttpUtility.HtmlDecode(_settings.Title)
			                   };

			if (!calendarItem.IsValid())
			{
				return null;
			}

			return calendarItem;
		}
	}
}