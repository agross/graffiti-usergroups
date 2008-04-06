using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Web;

using DnugLeipzig.Extensions.Configuration;
using DnugLeipzig.Plugins.Migration;

using Graffiti.Core;

namespace DnugLeipzig.Plugins
{
	public class EventPlugin : GraffitiEvent, IEventConfigurationSource
	{
		const string Form_CategoryName = "categoryName";
		const string Form_DateFormat = "dateFormat";
		const string Form_DefaultLocation = "defaultLocation";
		const string Form_EndDateField = "endDateField";
		const string Form_LocationField = "locationField";
		const string Form_LocationUnknownField = "locationUnknown";
		const string Form_MigrateFieldValues = "migrate";
		const string Form_RegistrationNeededField = "registrationNeeded";
		const string Form_ShortEndDateFormat = "shortDateFormat";
		const string Form_SpeakerField = "speakerField";
		const string Form_StartDateField = "startDateField";
		const string Form_UnknownText = "unknownText";
		const string Form_YearQueryString = "yearQueryString";

		public EventPlugin()
		{
			// Initialize with default values.
			CategoryName = "Talks";
			StartDateField = "Start Date";
			EndDateField = "End Date";
			SpeakerField = "Speaker";
			DateFormat = "on {0:D}, at {0:t}";
			ShortEndDateFormat = "at {0:t}";
			LocationField = "Location";
			UnknownText = "(TBA)";
			YearQueryString = "year";
			LocationUnknownField = "Location is unknown";
			RegistrationNeededField = "Registration needed";
		}

		public override string Name
		{
			get { return "Events Plugin"; }
		}

		public override bool IsEditable
		{
			get { return true; }
		}

		public override string Description
		{
			get { return "Extends Graffiti CMS for events management."; }
		}

		#region IEventConfigurationSource Members
		public string SortRelevantDateField
		{
			get { return StartDateField; }
		}

		public string CategoryName
		{
			get;
			set;
		}

		public string SpeakerField
		{
			get;
			set;
		}

		public string YearQueryString
		{
			get;
			set;
		}

		public string DateFormat
		{
			get;
			set;
		}

		public string EndDateField
		{
			get;
			set;
		}

		public string LocationField
		{
			get;
			set;
		}

		public string ShortEndDateFormat
		{
			get;
			set;
		}

		public string StartDateField
		{
			get;
			set;
		}

		public string UnknownText
		{
			get;
			set;
		}

		public string LocationUnknownField
		{
			get;
			set;
		}

		public string RegistrationNeededField
		{
			get;
			set;
		}
		#endregion

		public override void Init(GraffitiApplication ga)
		{
			Debug.WriteLine("Init Event Plugin");

			ga.BeforeValidate += ga_BeforeValidate;
			ga.BeforeInsert += ga_BeforeInsert;
			ga.BeforeUpdate += ga_BeforeUpdate;
		}

		void ga_BeforeValidate(DataBuddyBase dataObject, EventArgs e)
		{
		}

		void ga_BeforeUpdate(DataBuddyBase dataObject, EventArgs e)
		{
		}

		void ga_BeforeInsert(DataBuddyBase dataObject, EventArgs e)
		{
		}

		#region Settings
		protected override FormElementCollection AddFormElements()
		{
			return new FormElementCollection
			       {
			       	new CheckFormElement(Form_MigrateFieldValues,
			       	                     "Migrate custom field values",
			       	                     "Check to automatically migrate custom field values if category and/or field names change.",
			       	                     true),
			       	new TextFormElement(Form_CategoryName,
			       	                    "Graffiti events category",
			       	                    "Enter the name of the category to store events, e.g. \"Talks\"."),
			       	new TextFormElement(Form_StartDateField,
			       	                    "\"Event start date and time\" field",
			       	                    "Enter the name of the custom text field to store the events's start date (and, optionally, time), e.g. \"Start Date\". This field will be validated to be either empty or hold a correct date time/value."),
			       	new TextFormElement(Form_EndDateField,
			       	                    "\"Event end date and time\" field",
			       	                    "Enter the name of the custom text field to store the events's end date (and, optionally, time), e.g. \"End Date\". This field will be validated to be either empty or hold a correct date time/value."),
			       	new TextFormElement(Form_SpeakerField,
			       	                    "\"Speaker\" field",
			       	                    "Enter the name of the custom text field to store the speaker's name, e.g. \"Speaker\"."),
			       	new TextFormElement(Form_DateFormat,
			       	                    "Date/time format",
			       	                    "Enter .NET format string to be used for the start date and the end date, e.g. \"on {0:D}, at {0:t}\". Leave blank to use the Graffiti date format from web.config."),
			       	new TextFormElement(Form_ShortEndDateFormat,
			       	                    "Short end date/time format",
			       	                    "Enter .NET format string to be used for the end date if the start date and end date of the event is the same day, e.g. \"at {0:t}\". Leave blank to use the Graffiti date format from above."),
			       	new TextFormElement(Form_LocationField,
			       	                    "Event location field",
			       	                    "Enter the name of the custom text field to store the event's location, e.g. \"Location\"."),
			       	new TextFormElement(Form_LocationUnknownField,
			       	                    "\"Location unknown\" field",
			       	                    "Enter the name of the custom checkbox field to store if the event location is unknown, e.g. \"Location is unknown\"."),
			       	new TextFormElement(Form_DefaultLocation,
			       	                    "Default event location",
			       	                    "Enter the default value of the locaton if you don't enter one, e.g. \"Initech Corp., Floor 1\". This value will be used to fill the event location field above if \"Location unknown\" is not checked."),
			       	new TextFormElement(Form_UnknownText,
			       	                    "\"Unknown\" text",
			       	                    "Enter the text to be displayed if event information (dates, speaker, location) is not yet known, e.g. \"(TBA)\"."),
			       	new TextFormElement(Form_RegistrationNeededField,
			       	                    "\"Registration needed\" field",
			       	                    "Enter the name of the custom field to store if registration for the event is needed, e.g. \"Registration needed\"."),
			       	new TextFormElement(Form_YearQueryString,
			       	                    "Query string parameter for paging by year",
			       	                    "Enter a value for the query string parameter used to display talks of a specific year.")
			       };
		}

		public override StatusType SetValues(HttpContext context, NameValueCollection nvc)
		{
			try
			{
				HttpContext.Current.Cache.Remove(EventPluginConfigurationSource.CacheKey);

				if (String.IsNullOrEmpty(nvc[Form_CategoryName].Trim()))
				{
					SetMessage(context, "Please enter a category name.");
					return StatusType.Error;
				}

				string categoryName = HttpUtility.HtmlEncode(nvc[Form_CategoryName].Trim());
				if (!Util.StringToBoolean(nvc[Form_MigrateFieldValues]) && !Util.ValidateExistingCategory(categoryName))
				{
					SetMessage(context, String.Format("The category '{0}' does not exist.", categoryName));
					return StatusType.Warning;
				}

				if (String.IsNullOrEmpty(nvc[Form_YearQueryString]))
				{
					SetMessage(context, "Please enter a year query string parameter.");
					return StatusType.Error;
				}

				EventPluginMemento oldState = CreateMemento();

				CategoryName = categoryName;
				StartDateField = nvc[Form_StartDateField];
				EndDateField = nvc[Form_EndDateField];
				SpeakerField = nvc[Form_SpeakerField];
				DateFormat = nvc[Form_DateFormat];
				ShortEndDateFormat = nvc[Form_ShortEndDateFormat];
				LocationField = nvc[Form_LocationField];
				UnknownText = nvc[Form_UnknownText];
				LocationUnknownField = nvc[Form_LocationUnknownField];
				RegistrationNeededField = nvc[Form_RegistrationNeededField];
				YearQueryString = nvc[Form_YearQueryString];

				EventPluginMemento newState = CreateMemento();

				FieldMigrator migrator = new FieldMigrator();
				migrator.MigrateValues(new MigrationInfo(oldState, newState));

				return StatusType.Success;
			}
			catch (Exception ex)
			{
				SetMessage(context, String.Format("Error: {0}", ex.Message));
				return StatusType.Error;
			}
		}

		protected override NameValueCollection DataAsNameValueCollection()
		{
			var values = new NameValueCollection();

			values[Form_CategoryName] = HttpUtility.HtmlDecode(CategoryName);
			values[Form_StartDateField] = StartDateField;
			values[Form_EndDateField] = EndDateField;
			values[Form_SpeakerField] = SpeakerField;
			values[Form_DateFormat] = DateFormat;
			values[Form_ShortEndDateFormat] = ShortEndDateFormat;
			values[Form_LocationField] = LocationField;
			values[Form_UnknownText] = UnknownText;
			values[Form_LocationUnknownField] = LocationUnknownField;
			values[Form_RegistrationNeededField] = RegistrationNeededField;
			values[Form_YearQueryString] = YearQueryString;

			return values;
		}
		#endregion

		#region Memento
		EventPluginMemento CreateMemento()
		{
			return new EventPluginMemento(this);
		}
		#endregion
	}
}