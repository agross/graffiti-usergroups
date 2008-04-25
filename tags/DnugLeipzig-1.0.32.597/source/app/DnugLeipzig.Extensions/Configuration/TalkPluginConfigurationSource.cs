using System;
using System.Diagnostics;
using System.Web;
using System.Web.Caching;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Configuration
{
	public class TalkPluginConfigurationSource : ITalkConfigurationSource
	{
		public static readonly string CacheKey = typeof(TalkPluginConfigurationSource).Name;
		static ITalkConfigurationSource PluginInstance;

		#region ITalkConfigurationSource Members
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
			Debug.WriteLine("TalkPluginConfigurationSource.EnsureCurrentInstance");

			PluginInstance = HttpContext.Current.Cache.Get(CacheKey) as ITalkConfigurationSource;
			if (PluginInstance != null)
			{
				Debug.WriteLine("--> Cached");
				return;
			}

			Debug.WriteLine("--> Not cached");

			// Ensure Plugin initialization occurs before we query the Plugin settings.
			Events.Instance();

			PluginInstance =
				Events.GetEvent("DnugLeipzig.Plugins.TalkPlugin, DnugLeipzig.Plugins").Event as ITalkConfigurationSource;

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