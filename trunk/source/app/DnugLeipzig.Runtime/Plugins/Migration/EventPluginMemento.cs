using System;
using System.Collections.Generic;

using DnugLeipzig.Definitions.Plugins.Events;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Plugins.Migration
{
	internal class EventPluginMemento : IMemento
	{
		public EventPluginMemento(IEventPluginConfigurationProvider source)
		{
			SpeakerField = source.SpeakerField;
			EndDateField = source.EndDateField;
			LocationField = source.LocationField;
			StartDateField = source.StartDateField;
			LocationUnknownField = source.LocationUnknownField;
			RegistrationNeededField = source.RegistrationNeededField;
			RegistrationRecipientField = source.RegistrationRecipientField;
			MaximumNumberOfRegistrationsField = source.MaximumNumberOfRegistrationsField;
			RegistrationListField = source.RegistrationListField;
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

		public string RegistrationRecipientField
		{
			get;
			protected set;
		}

		public string MaximumNumberOfRegistrationsField
		{
			get;
			protected set;
		}

		public string RegistrationListField
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
				       		new Guid("{01E8A9D9-AEB0-472d-BE17-5CE9D50DBBC0}"),
				       		new FieldInfo(StartDateField, FieldType.TextBox, "Start date (and, optionally, time) of the event.")
				       		},
				       	{
				       		new Guid("{385FC37F-3D5C-4790-829B-43BD1E64BB4B}"),
				       		new FieldInfo(EndDateField, FieldType.TextBox, "End date (and, optionally, time) of the event.")
				       		},
				       	{
				       		new Guid("{7B411215-E069-4f5b-AEA9-F9EC70A68B51}"),
				       		new FieldInfo(SpeakerField, FieldType.TextBox, "Event speaker(s).")
				       		},
				       	{
				       		new Guid("{51BE26B6-03FC-47de-A490-283359C4C47A}"),
				       		new FieldInfo(LocationUnknownField, FieldType.CheckBox, "Check if the location is not known yet.")
				       		},
				       	{
				       		new Guid("{AF99664E-BB66-446a-8968-6899348EEB34}"),
				       		new FieldInfo(LocationField,
				       		              FieldType.TextBox,
				       		              "The event location. Leave empty to apply the default location set in the Event plugin configuration.")
				       		},
				       	{
				       		new Guid("{4DA43A0C-1C7E-4df6-A72A-0671660D8318}"),
				       		new FieldInfo(RegistrationNeededField,
				       		              FieldType.CheckBox,
				       		              "Check if attendees should sign up for the event using the registration page.")
				       		},
				       	{
				       		new Guid("{9151702C-88C9-48f2-8C2C-10C17264F456}"),
				       		new FieldInfo(RegistrationRecipientField,
				       		              FieldType.TextBox,
				       		              "The e-mail address to receive registration requests. Leave empty to apply the default e-mail address set in the Event plugin configuration.")
				       		},
				       	{
				       		new Guid("{FB6BF2D1-28C7-428e-8ABF-357D7EE82EB3}"),
				       		new FieldInfo(MaximumNumberOfRegistrationsField,
				       		              FieldType.TextBox,
				       		              "The maximum number of registrations before registrations are put on the waiting list. Leave empty to apply the default maximum number of registrations set in the Event plugin configuration.")
				       		},
				       	{
				       		new Guid("{6741A2DA-6B1E-481c-B139-A0043AEC0EE0}"),
				       		new FieldInfo(RegistrationListField, FieldType.TextArea, "The list of registered attendees.")
				       		}
				       };
			}
		}
		#endregion
	}
}