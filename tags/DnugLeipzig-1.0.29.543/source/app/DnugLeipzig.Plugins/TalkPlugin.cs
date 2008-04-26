using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Web;

using DnugLeipzig.Extensions.Configuration;

using Graffiti.Core;

namespace DnugLeipzig.Plugins
{
	public class TalkPlugin : GraffitiEvent, ITalkConfigurationSource
	{
		const string Form_CategoryName = "categoryName";
		const string Form_DateField = "dateField";
		const string Form_SpeakerField = "speakerField";
		const string Form_YearQueryString = "yearQueryString";
		string _categoryName;
		string _DateField;
		string _speakerField;
		string _yearQueryString;

		public TalkPlugin()
		{
			// Initialize with default values.
			CategoryName = "Talks";
			DateField = "Date";
			SpeakerField = "Speaker";
			YearQueryString = "year";
		}

		public override string Name
		{
			get { return "Talks Plugin"; }
		}

		public override bool IsEditable
		{
			get { return true; }
		}

		public override string Description
		{
			get { return "Extends Graffiti CMS for talks management."; }
		}

		//{{{-ConvertToAutoProperty

		#region ITalkConfigurationSource Members
		public string DateField
		{
			get { return _DateField; }
			set { _DateField = value; }
		}

		public string SortRelevantDateField
		{
			get { return DateField; }
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
		#endregion

		//{{{+ConvertToAutoProperty

		public override void Init(GraffitiApplication ga)
		{
			Debug.WriteLine("Init Talk Plugin");

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
			       	                    "Graffiti talks category",
			       	                    "Enter the name of the category in which you store talks, e.g. \"Talks\"."),
			       	new TextFormElement(Form_DateField,
			       	                    "Talk date and time custom field",
			       	                    "Enter the name of the custom field in which the talk's date (and, optionally, time) is stored, e.g. \"Date\". This field will be validated to be either empty or hold a correct date time/value."),
			       	new TextFormElement(Form_SpeakerField,
			       	                    "Speaker custom field",
			       	                    "Enter the name of the custom field in which the speaker's name is stored, e.g. \"Speaker\"."),
			       	new TextFormElement(Form_YearQueryString,
			       	                    "Query string parameter for paging by year",
			       	                    "Enter a value for the query string parameter used to display talks of a specific year.")
			       };
		}

		public override StatusType SetValues(HttpContext context, NameValueCollection nvc)
		{
			HttpContext.Current.Cache.Remove(TalkPluginConfigurationSource.CacheKey);

			if (String.IsNullOrEmpty(nvc[Form_CategoryName]))
			{
				SetMessage(context, "Enter a category name.");
				return StatusType.Error;
			}

			CategoryName = HttpUtility.HtmlEncode(nvc[Form_CategoryName]);
			DateField = nvc[Form_DateField];
			SpeakerField = nvc[Form_SpeakerField];

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
			values[Form_DateField] = DateField;
			values[Form_SpeakerField] = SpeakerField;
			values[Form_YearQueryString] = YearQueryString;

			return values;
		}
		#endregion
	}
}