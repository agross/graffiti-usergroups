using System;

using DnugLeipzig.Definitions.Validation;
using DnugLeipzig.Runtime.Validation;

namespace DnugLeipzig.Runtime.Plugins.Events
{
	public static class EventPluginSettingsErrors
	{
		// TODO: English
		public static readonly INotification CategoryIsMissing = new ValidationError("Please enter a category name.");

		public static readonly INotification DefaultMaximumNumberOfRegistrationsMustBeAnInteger =
			new ValidationError("Please enter a valid integer value for the default maximum number of registrations.");

		public static readonly INotification DefaultMaximumNumberOfRegistrationsMustBeEqualOrGreaterThanZero =
			new ValidationError(
				"Please enter a value greater than or equal to 0 for the default maximum number of registrations.");

		public static readonly INotification DefaultRegistrationRecipientEmailIsInvalid =
			new ValidationError("Please enter a valid e-mail address for the default registration recipient.");

		public static readonly INotification YearQueryStringIsMissing =
			new ValidationError("Please enter a year query string parameter.");

		public static INotification CategoryDoesNotExist(EventPluginSettings instance)
		{
			return new ValidationWarning(String.Format("The category '{0}' does not exist.",
			                                           instance.CategoryName));
		}
	}
}