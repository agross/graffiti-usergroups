using System;
using System.Collections.Generic;

using DnugLeipzig.Definitions.Configuration.Plugins;

using Graffiti.Core;

namespace DnugLeipzig.Plugins.Migration
{
	internal class TalkPluginMemento : IMemento
	{
		public TalkPluginMemento(ITalkPluginConfigurationProvider source)
		{
			SpeakerField = source.SpeakerField;
			DateField = source.DateField;
			CategoryName = source.CategoryName;
		}

		public string DateField
		{
			get;
			protected set;
		}

		public string SpeakerField
		{
			get;
			protected set;
		}

		#region IMemento Members
		public string CategoryName
		{
			get;
			protected set;
		}

		public IDictionary<Guid, FieldInfo> Fields
		{
			get
			{
				return new Dictionary<Guid, FieldInfo>
				       {
				       	{
				       		new Guid("{7DEF5E58-C778-4d0d-A112-2CC9C5E4B5E0}"),
				       		new FieldInfo(DateField, FieldType.TextBox, "Date (and, optionally, time) of the talk.")
				       		},
				       	{
				       		new Guid("{463D826E-BAF8-4ebd-B401-FE6FABDBD38E}"),
				       		new FieldInfo(SpeakerField, FieldType.TextBox, "Event speaker(s).")
				       		}
				       };
			}
		}
		#endregion
	}
}