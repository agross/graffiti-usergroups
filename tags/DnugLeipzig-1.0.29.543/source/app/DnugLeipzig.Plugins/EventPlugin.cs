using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Web;

using DnugLeipzig.Extensions.Configuration;

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
		const string Form_ShortEndDateFormat = "shortDateFormat";
		const string Form_SpeakerField = "speakerField";
		const string Form_StartDateField = "startDateField";
		const string Form_UnknownText = "unknownText";
		const string Form_YearQueryString = "yearQueryString";
		string _categoryName;
		string _dateFormat;
		string _endDateField;
		string _locationField;
		string _shortEndDateFormat;
		string _speakerField;
		string _startDateField;
		string _unknownText;
		string _yearQueryString;

		public EventPlugin()
		{
			// Initialize with default values.
			CategoryName = "Talks";
			StartDateField = "Start Date";
			EndDateField = "End Date";
			SpeakerField = "Speaker";
			DateFormat = "{0:D}, {0:t}";
			ShortEndDateFormat = "{0:t}";
			LocationField = "Location";
			UnknownText = "(Unknown)";
			YearQueryString = "year";
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

		//{{{-ConvertToAutoProperty

		#region IEventConfigurationSource Members
		public string SortRelevantDateField
		{
			get { return StartDateField; }
		}

		public string CategoryName
		{
			get { return _categoryName; }
			set { _categoryName = value; }
		}

		public string SpeakerField
		{
			get { return _speakerField; }
			set { _speakerField = value; }
		}

		public string YearQueryString
		{
			get { return _yearQueryString; }
			set { _yearQueryString = value; }
		}

		public string DateFormat
		{
			get { return _dateFormat; }
			set { _dateFormat = value; }
		}

		public string EndDateField
		{
			get { return _endDateField; }
			set { _endDateField = value; }
		}

		public string LocationField
		{
			get { return _locationField; }
			set { _locationField = value; }
		}

		public string ShortEndDateFormat
		{
			get { return _shortEndDateFormat; }
			set { _shortEndDateFormat = value; }
		}

		public string StartDateField
		{
			get { return _startDateField; }
			set { _startDateField = value; }
		}

		public string UnknownText
		{
			get { return _unknownText; }
			set { _unknownText = value; }
		}
		#endregion

		//{{{+ConvertToAutoProperty

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
			       	new TextFormElement(Form_CategoryName,
			       	                    "Graffiti events category",
			       	                    "Enter the name of the category in which you store talks, e.g. \"Talks\"."),
			       	new TextFormElement(Form_StartDateField,
			       	                    "Event start date and time custom field",
			       	                    "Enter the name of the custom field in which the events's start date (and, optionally, time) is stored, e.g. \"Start Date\". This field will be validated to be either empty or hold a correct date time/value."),
			       	new TextFormElement(Form_EndDateField,
			       	                    "Event end date and time custom field",
			       	                    "Enter the name of the custom field in which the events's end date (and, optionally, time) is stored, e.g. \"End Date\". This field will be validated to be either empty or hold a correct date time/value."),
			       	new TextFormElement(Form_SpeakerField,
			       	                    "Speaker custom field",
			       	                    "Enter the name of the custom field in which the speaker's name is stored, e.g. \"Speaker\"."),
			       	new TextFormElement(Form_DateFormat,
			       	                    "Date/time format",
			       	                    "Enter .NET format string to be used for the start date and the end date, e.g. \"{0:D}, {0:t}\". Leave blank to use the Graffiti date format from web.config."),
			       	new TextFormElement(Form_ShortEndDateFormat,
			       	                    "Short end date/time format",
			       	                    "Enter .NET format string to be used for the end date if the start date and end date of the event is the same day, e.g. \"{0:t}\". Leave blank to use the Graffiti date format from above."),
			       	new TextFormElement(Form_LocationField,
			       	                    "Event location",
			       	                    "Enter the name of the custom field in which the events location is stored, e.g. \"Location\"."),
			       	new TextFormElement(Form_DefaultLocation,
			       	                    "Default event location",
			       	                    "Enter the default value of the locaton if you don't enter one, e.g. \"Initech Corp., Floor 1\". This value will be used to fill the event location field above."),
			       	new TextFormElement(Form_UnknownText,
			       	                    "\"Unknown\" text",
			       	                    "Enter the text to be displayed if event information (dates, speaker, location) is not yet known, e.g. \"(Unknown)\"."),
			       	new TextFormElement(Form_YearQueryString,
			       	                    "Query string parameter for paging by year",
			       	                    "Enter a value for the query string parameter used to display talks of a specific year.")
			       };
		}

		public override StatusType SetValues(HttpContext context, NameValueCollection nvc)
		{
			HttpContext.Current.Cache.Remove(EventPluginConfigurationSource.CacheKey);

			if (String.IsNullOrEmpty(nvc[Form_CategoryName]))
			{
				SetMessage(context, "Enter a category name.");
				return StatusType.Error;
			}

			CategoryName = HttpUtility.HtmlEncode(nvc[Form_CategoryName]);
			StartDateField = nvc[Form_StartDateField];
			EndDateField = nvc[Form_EndDateField];
			SpeakerField = nvc[Form_SpeakerField];
			DateFormat = nvc[Form_DateFormat];
			ShortEndDateFormat = nvc[Form_ShortEndDateFormat];
			LocationField = nvc[Form_LocationField];
			UnknownText = nvc[Form_UnknownText];

			if (String.IsNullOrEmpty(nvc[Form_YearQueryString]))
			{
				SetMessage(context, "Enter a year query string parameter.");
				return StatusType.Error;
			}
			YearQueryString = nvc[Form_YearQueryString];

			return StatusType.Success;
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
			values[Form_YearQueryString] = YearQueryString;

			return values;
		}
		#endregion
	}
}