using System;
using System.Collections.Generic;

using DnugLeipzig.Definitions.Configuration;

using Graffiti.Core;

namespace DnugLeipzig.Plugins.Migration
{
	internal class TalkPluginMemento : IMemento
	{
		public TalkPluginMemento(ITalkPluginConfiguration source)
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

		public HashSet<FieldInfo> Fields
		{
			get
			{
				return new HashSet<FieldInfo>
				       {
				       	new FieldInfo(DateField, FieldType.TextBox, "The date of the talk."),
				       	new FieldInfo(SpeakerField, FieldType.TextBox, "Talk speaker(s).")
				       };
			}
		}
		#endregion
	}
}