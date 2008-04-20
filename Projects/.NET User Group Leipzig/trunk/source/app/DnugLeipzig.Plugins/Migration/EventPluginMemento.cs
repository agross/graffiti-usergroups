using System.Collections.Generic;

using DnugLeipzig.Definitions.Configuration;

using Graffiti.Core;

namespace DnugLeipzig.Plugins.Migration
{
	internal class EventPluginMemento : IMemento
	{
		public EventPluginMemento(IEventPluginConfigurationSource source)
		{
			SpeakerField = source.SpeakerField;
			EndDateField = source.EndDateField;
			LocationField = source.LocationField;
			StartDateField = source.StartDateField;
			LocationUnknownField = source.LocationUnknownField;
			RegistrationNeededField = source.RegistrationNeededField;
			CategoryName = source.CategoryName;
		}

		public string SpeakerField
		{
			get;
			protected set;
		}

		public string EndDateField
		{
			get;
			protected set;
		}

		public string LocationField
		{
			get;
			protected set;
		}

		public string StartDateField
		{
			get;
			protected set;
		}

		public string LocationUnknownField
		{
			get;
			protected set;
		}

		public string RegistrationNeededField
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

		public Dictionary<string, FieldInfo> Fields
		{
			get
			{
				return new Dictionary<string, FieldInfo>
				       {
				       	{ "{01E8A9D9-AEB0-472d-BE17-5CE9D50DBBC0}", new FieldInfo(StartDateField, FieldType.TextBox) },
				       	{ "{385FC37F-3D5C-4790-829B-43BD1E64BB4B}", new FieldInfo(EndDateField, FieldType.TextBox) },
				       	{ "{7B411215-E069-4f5b-AEA9-F9EC70A68B51}", new FieldInfo(SpeakerField, FieldType.TextBox) },
				       	{ "{51BE26B6-03FC-47de-A490-283359C4C47A}", new FieldInfo(LocationUnknownField, FieldType.CheckBox) },
				       	{ "{AF99664E-BB66-446a-8968-6899348EEB34}", new FieldInfo(LocationField, FieldType.TextBox) },
				       	{ "{4DA43A0C-1C7E-4df6-A72A-0671660D8318}", new FieldInfo(RegistrationNeededField, FieldType.CheckBox) }
				       };
			}
		}
		#endregion
	}
}