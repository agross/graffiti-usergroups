using Graffiti.Core;

namespace DnugLeipzig.Runtime.Plugins.Talks
{
	public partial class TalkPlugin
	{
		#region Nested type: Fields
		internal static class Fields
		{
			internal const string CategoryName = "categoryName";
			internal const string CreateTargetCategoryAndFields = "createTargetCategoryAndFields";
			internal const string Date = "dateField";
			internal const string MigrateFieldValues = "migrate";
			internal const string Speaker = "speakerField";
			internal const string YearQueryString = "yearQueryString";

			public static FormElementCollection AsFormElements()
			{
				return new FormElementCollection
				       {
				       	new CheckFormElement(CreateTargetCategoryAndFields,
				       	                     "Create category and fields",
				       	                     "Check to automatically create the category and custom fields.",
				       	                     true),
				       	new CheckFormElement(MigrateFieldValues,
				       	                     "Migrate custom field values",
				       	                     "Check to automatically migrate posts and custom field values if category and/or field names change.",
				       	                     true),
				       	new TextFormElement(CategoryName,
				       	                    "Graffiti talks category",
				       	                    "Enter the name of the category in which you store talks, e.g. \"Talks\"."),
				       	new TextFormElement(Date,
				       	                    "Talk date and time custom field",
				       	                    "Enter the name of the custom field in which the talk's date (and, optionally, time) is stored, e.g. \"Date\". This field will be validated to be either empty or hold a correct date time/value."),
				       	new TextFormElement(Speaker,
				       	                    "Speaker custom field",
				       	                    "Enter the name of the custom field in which the speaker's name is stored, e.g. \"Speaker\"."),
				       	new TextFormElement(YearQueryString,
				       	                    "Query string parameter for paging by year",
				       	                    "Enter a value for the query string parameter used to display talks of a specific year.")
				       };
			}
		}
		#endregion
	}
}