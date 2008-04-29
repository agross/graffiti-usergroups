using System;
using System.Diagnostics;
using System.Web;
using System.Web.Caching;

using Graffiti.Core;

namespace DnugLeipzig.Definitions.Configuration
{
	public class TalkPluginConfiguration : ITalkPluginConfiguration
	{
		public static readonly string CacheKey = typeof(TalkPluginConfiguration).Name;
		static ITalkPluginConfiguration PluginInstance;

		#region ITalkPluginConfiguration Members
		public string DateField
		{
			get
			{
				EnsureCurrentInstance();
				return PluginInstance.DateField;
			}
		}

		public string CategoryName
		{
			get
			{
				EnsureCurrentInstance();
				return PluginInstance.CategoryName;
			}
		}

		public string SpeakerField
		{
			get
			{
				EnsureCurrentInstance();
				return PluginInstance.SpeakerField;
			}
		}

		public string YearQueryString
		{
			get
			{
				EnsureCurrentInstance();
				return PluginInstance.YearQueryString;
			}
		}

		public string SortRelevantDateField
		{
			get
			{
				EnsureCurrentInstance();
				return PluginInstance.SortRelevantDateField;
			}
		}
		#endregion

		static void EnsureCurrentInstance()
		{
			Debug.WriteLine("TalkPluginConfiguration.EnsureCurrentInstance");

			PluginInstance = HttpContext.Current.Cache.Get(CacheKey) as ITalkPluginConfiguration;
			if (PluginInstance != null)
			{
				Debug.WriteLine("--> Cached");
				return;
			}

			Debug.WriteLine("--> Not cached");

			// Ensure plugin initialization occurs before we query the plugin settings.
			Events.Instance();

			EventDetails eventDetails = Events.GetEvent("DnugLeipzig.Plugins.TalkPlugin, DnugLeipzig.Plugins");
			if(!eventDetails.Enabled)
			{
				throw new InvalidOperationException("The Talks Plugin has not been enabled.");
			}

			PluginInstance = eventDetails.Event as ITalkPluginConfiguration;

			HttpContext.Current.Cache.Add(CacheKey,
			                              PluginInstance,
			                              null,
			                              Cache.NoAbsoluteExpiration,
			                              Cache.NoSlidingExpiration,
			                              CacheItemPriority.NotRemovable,
			                              null);
		}
	}
}