using Graffiti.Core;

namespace DnugLeipzig.Runtime.Plugins.Events
{
	public partial class EventPlugin
	{
		#region Nested type: Fields
		internal static class Fields
		{
			internal const string CategoryName = "categoryName";
			internal const string CreateTargetCategoryAndFields = "createTargetCategoryAndFields";
			internal const string DateFormat = "dateFormat";
			internal const string DefaultLocation = "defaultLocation";
			internal const string DefaultMaximumNumberOfRegistrations = "defaultMaximumNumberOfRegistrations";
			internal const string DefaultRegistrationRecipient = "defaultRegistrationRecipient";
			internal const string EndDate = "endDateField";
			internal const string Location = "locationField";
			internal const string LocationUnknown = "locationUnknown";
			internal const string MaximumNumberOfRegistrations = "maximumNumberOfRegistrations";
			internal const string MigrateFieldValues = "migrate";
			internal const string RegistrationList = "numberOfRegistrations";
			internal const string RegistrationMailSubject = "registrationMailSubject";
			internal const string RegistrationNeeded = "registrationNeeded";
			internal const string RegistrationRecipient = "registrationRecipient";
			internal const string ShortEndDateFormat = "shortDateFormat";
			internal const string Speaker = "speakerField";
			internal const string StartDate = "startDateField";
			internal const string UnknownText = "unknownText";
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
				       	                    "Graffiti events category",
				       	                    "Enter the name of the category to store events, e.g. \"Events\"."),
				       	new TextFormElement(StartDate,
				       	                    "\"Event start date and time\" field",
				       	                    "Enter the name of the custom text field to store the events's start date (and, optionally, time), e.g. \"Start Date\". This field will be validated to be either empty or hold a correct date time/value."),
				       	new TextFormElement(EndDate,
				       	                    "\"Event end date and time\" field",
				       	                    "Enter the name of the custom text field to store the events's end date (and, optionally, time), e.g. \"End Date\". This field will be validated to be either empty or hold a correct date time/value."),
				       	new TextFormElement(Speaker,
				       	                    "\"Speaker\" field",
				       	                    "Enter the name of the custom text field to store the speaker's name, e.g. \"Speaker\"."),
				       	new TextFormElement(DateFormat,
				       	                    "Date/time format",
				       	                    "Enter .NET format string to be used for the start date and the end date, e.g. \"on {0:D}, at {0:t}\". Leave blank to use the Graffiti date format from web.config."),
				       	new TextFormElement(ShortEndDateFormat,
				       	                    "Short end date/time format",
				       	                    "Enter .NET format string to be used for the end date if the start date and end date of the event is the same day, e.g. \"at {0:t}\". Leave blank to use the Graffiti date format from above."),
				       	new TextFormElement(Location,
				       	                    "Event location field",
				       	                    "Enter the name of the custom text field to store the event's location, e.g. \"Location\"."),
				       	new TextFormElement(LocationUnknown,
				       	                    "\"Location unknown\" field",
				       	                    "Enter the name of the custom checkbox field to store if the event location is unknown, e.g. \"Location is unknown\"."),
				       	new TextFormElement(DefaultLocation,
				       	                    "Default event location",
				       	                    "Enter the default value of the locaton if you don't enter one, e.g. \"Initech Corp., Floor 1\". This value will be used to fill the event location field above if \"Location unknown\" is not checked."),
				       	new TextFormElement(UnknownText,
				       	                    "\"Unknown\" text",
				       	                    "Enter the text to be displayed if event information (dates, speaker, location) is not yet known, e.g. \"(To be announced)\"."),
				       	new TextFormElement(RegistrationNeeded,
				       	                    "\"Registration needed\" field",
				       	                    "Enter the name of the custom checkbox field to store if registration for the event is needed, e.g. \"Registration needed\"."),
				       	new TextFormElement(RegistrationRecipient,
				       	                    "\"Registration recipient e-mail address\" field",
				       	                    "Enter the name of the custom textbox field to store the registration recipient e-mail address, e.g. \"Registration recipient e-mail address\"."),
				       	new TextFormElement(DefaultRegistrationRecipient,
				       	                    "Default registration recipient e-mail address",
				       	                    "Enter the default registration e-mail address, e.g. \"registration@example.com\"."),
				       	new TextFormElement(RegistrationMailSubject,
				       	                    "Registration e-mail subject",
				       	                    "Enter the registration e-mail subject, e.g. \"New Registration\"."),
				       	new TextFormElement(MaximumNumberOfRegistrations,
				       	                    "\"Maximum number of registrations\" field",
				       	                    "Enter the name of the custom textbox field to store the maximum number of registrations for the event, e.g. \"Maximum number of registrations\"."),
				       	new TextFormElement(DefaultMaximumNumberOfRegistrations,
				       	                    "Default maximum number of registrations",
				       	                    "Enter the default maximum number of registrations, e.g. \"100\". If that number is reached, new registrations will not be possible. Leave blank to allow unlimited registrations."),
				       	new TextFormElement(RegistrationList,
				       	                    "\"Registration list\" field",
				       	                    "Enter the name of the custom multi-line textbox field to store registration information like the attendees' e-mail addresses, e.g. \"Registration list\"."),
				       	new TextFormElement(YearQueryString,
				       	                    "Query string parameter for paging by year",
				       	                    "Enter a value for the query string parameter used to display talks of a specific year.")
				       };
			}
		}
		#endregion
	}
}