using DnugLeipzig.Definitions.Validation;
using DnugLeipzig.Runtime.Validation;

namespace DnugLeipzig.Runtime.Plugins.Events.Validation
{
	public static class EventErrors
	{
		// TODO: English
		public static INotification InvalidDate(string field)
		{
			return new ValidationError("Please enter a valid date.", field);
		}

		public static INotification StartDateIsRequiredWhenEndDateIsSet(string field)
		{
			return new ValidationError("Please enter a start date if the end date is set.", field);
		}

		public static INotification StartDateMustBeBeforeEndDate(string startDateField, string endDateField)
		{
			return new ValidationError("Please enter a start date that is less or equal than the end date.",
			                           startDateField,
			                           endDateField);
		}

		public static INotification MaximumNumberOfRegistrationsMustBeEqualOrGreaterThanZero(string field)
		{
			return new ValidationError("Please enter a value greater or equal than 0.", field);
		}

		public static INotification InvalidRegistrationRecipientEmail(string field)
		{
			return new ValidationError("Please enter a valid e-mail address.", field);
		}
	}
}