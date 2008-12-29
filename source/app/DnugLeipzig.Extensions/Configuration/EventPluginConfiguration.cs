using DnugLeipzig.Definitions.Configuration.Plugins;
using DnugLeipzig.Plugins;

namespace DnugLeipzig.Extensions.Configuration
{
	public class EventPluginConfiguration : PluginConfiguration<EventPlugin>, IEventPluginConfiguration
	{
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

		public string RegistrationListField
		{
			get
			{
				EnsureCurrentInstance();
				return PluginInstance.RegistrationListField;
			}
		}

		public string RegistrationMailSubject
		{
			get
			{
				EnsureCurrentInstance();
				return PluginInstance.RegistrationMailSubject;
			}
		}
		#endregion
	}
}