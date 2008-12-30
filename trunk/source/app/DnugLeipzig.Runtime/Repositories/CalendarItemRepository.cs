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
		readonly IGraffitiSiteSettings _settings;

		public CalendarItemRepository(IEventPluginConfiguration eventPluginConfiguration,
		                              IGraffitiSiteSettings settings)
		{
			_eventPluginConfiguration = eventPluginConfiguration;
			_settings = settings;
		}

		#region Implementation of ICalendarItemRepository
		public ICalendarItem CreateCalendarItemForEvent(Post post)
		{
			try
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
					// HACK: In unit-testing scenarios, accessing URL causes database access. Well done, telligent.
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
			catch (Exception ex)
			{
				Logger.Error(Create.New.Message().WithTitle("Could not create calendar item from post {0}", post.Id), ex);
				return null;
			}
		}
		#endregion
	}
}