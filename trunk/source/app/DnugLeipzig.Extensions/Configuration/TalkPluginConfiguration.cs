using System;

using DnugLeipzig.Definitions.Configuration;
using DnugLeipzig.Plugins;

namespace DnugLeipzig.Extensions.Configuration
{
	public class TalkPluginConfiguration : PluginConfiguration<TalkPlugin>, ITalkPluginConfiguration
	{
		//protected override string CacheKey
		//{
		//    get { return String.Format("{0}-BFCE9E46-61C8-4686-A0C8-F0750E710359", GetType().Name); }
		//}

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