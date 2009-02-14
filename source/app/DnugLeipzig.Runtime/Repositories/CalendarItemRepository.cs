using System;
using System.Web;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Extensions;
using DnugLeipzig.Definitions.GraffitiIntegration;
using DnugLeipzig.Definitions.Plugins.Events;
using DnugLeipzig.Definitions.Repositories;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Repositories
{
	public class CalendarItemRepository : Repository, ICalendarItemRepository
	{
		readonly IEventPluginConfigurationProvider _eventPluginConfigurationProvider;
		readonly IGraffitiSiteSettings _settings;

		public CalendarItemRepository(IEventPluginConfigurationProvider eventPluginConfigurationProvider,
		                              IGraffitiSiteSettings settings)
		{
			_eventPluginConfigurationProvider = eventPluginConfigurationProvider;
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
					// HACK: In unit-testing scenarios, accessing URL causes database access. Well done, telligent!
				}

				var calendarItem = new CalendarItem
				                   {
				                   	StartDate = post[_eventPluginConfigurationProvider.StartDateField].AsEventDate(),
				                   	EndDate = post[_eventPluginConfigurationProvider.EndDateField].AsEventDate(),
				                   	Location = post[_eventPluginConfigurationProvider.LocationUnknownField].IsSelected()
				                   	           	? _eventPluginConfigurationProvider.UnknownText
				                   	           	: post[_eventPluginConfigurationProvider.LocationField],
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
				Logger.Error(Create.New.LogMessage().WithTitle("Could not create calendar item from post {0}", post.Id), ex);
				return null;
			}
		}
		#endregion
	}
}