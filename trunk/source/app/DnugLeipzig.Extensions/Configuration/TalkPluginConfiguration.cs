using DnugLeipzig.Definitions.Configuration.Plugins;
using DnugLeipzig.Plugins;

namespace DnugLeipzig.Extensions.Configuration
{
	public class TalkPluginConfiguration : PluginConfiguration<TalkPlugin>, ITalkPluginConfiguration
	{
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
	}
}