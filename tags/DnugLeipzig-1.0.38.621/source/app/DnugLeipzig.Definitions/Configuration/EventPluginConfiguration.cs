using System.Diagnostics;
using System.Web;
using System.Web.Caching;

using Graffiti.Core;

namespace DnugLeipzig.Definitions.Configuration
{
	public class EventPluginConfiguration : IEventPluginConfiguration
	{
		public static readonly string CacheKey = typeof(EventPluginConfiguration).Name;
		static IEventPluginConfiguration PluginInstance;

		#region IEventPluginConfiguration Members
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

		public string DateFormat
		{
			get
			{
				EnsureCurrentInstance();
				return PluginInstance.DateFormat;
			}
		}

		public string EndDateField
		{
			get
			{
				EnsureCurrentInstance();
				return PluginInstance.EndDateField;
			}
		}

		public string LocationField
		{
			get
			{
				EnsureCurrentInstance();
				return PluginInstance.LocationField;
			}
		}

		public string ShortEndDateFormat
		{
			get
			{
				EnsureCurrentInstance();
				return PluginInstance.ShortEndDateFormat;
			}
		}

		public string StartDateField
		{
			get
			{
				EnsureCurrentInstance();
				return PluginInstance.StartDateField;
			}
		}

		public string UnknownText
		{
			get
			{
				EnsureCurrentInstance();
				return PluginInstance.UnknownText;
			}
		}

		public string LocationUnknownField
		{
			get
			{
				EnsureCurrentInstance();
				return PluginInstance.LocationUnknownField;
			}
		}

		public string RegistrationNeededField
		{
			get
			{
				EnsureCurrentInstance();
				return PluginInstance.RegistrationNeededField;
			}
		}

		public string RegistrationRecipientField
		{
			get
			{
				EnsureCurrentInstance();
				return PluginInstance.RegistrationRecipientField;
			}
		}

		public string MaximumNumberOfRegistrationsField
		{
			get
			{
				EnsureCurrentInstance();
				return PluginInstance.MaximumNumberOfRegistrationsField;
			}
		}

		public string NumberOfRegistrationsField
		{
			get
			{
				EnsureCurrentInstance();
				return PluginInstance.NumberOfRegistrationsField;
			}
		}
		#endregion

		static void EnsureCurrentInstance()
		{
			Debug.WriteLine("EventPluginConfiguration.EnsureCurrentInstance");

			PluginInstance = HttpContext.Current.Cache.Get(CacheKey) as IEventPluginConfiguration;
			if (PluginInstance != null)
			{
				Debug.WriteLine("--> Cached");
				return;
			}

			Debug.WriteLine("--> Not cached");

			// Ensure Plugin initialization occurs before we query the Plugin settings.
			Events.Instance();

			PluginInstance =
				Events.GetEvent("DnugLeipzig.Plugins.EventPlugin, DnugLeipzig.Plugins").Event as IEventPluginConfiguration;

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