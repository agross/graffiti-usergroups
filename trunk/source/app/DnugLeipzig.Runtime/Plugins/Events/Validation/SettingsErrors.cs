using System;

using DnugLeipzig.Definitions.Validation;
using DnugLeipzig.Runtime.Validation;

namespace DnugLeipzig.Runtime.Plugins.Events.Validation
{
	public static class SettingsErrors
	{
		// TODO: English
		public static readonly INotification CategoryIsMissing = new ValidationError("Please enter a category name.");

		public static readonly INotification DefaultMaximumNumberOfRegistrationsMustEqualOrGreaterThanZero =
			new ValidationError(
				"Please enter a value greater than or equal to 0 for the default maximum number of registrations.");

		public static readonly INotification InvalidDefaultRegistrationRecipientEmail =
			new ValidationError("Please enter a valid e-mail address for the default registration recipient.");

		public static readonly INotification YearQueryStringIsMissing =
			new ValidationError("Please enter a year query string parameter.");

		public static INotification EarliestRegistrationFieldIsMissing =
			new ValidationError("Please enter a value for the earliest registration field.");
	
		public static INotification LatestRegistrationFieldIsMissing =
			new ValidationError("Please enter a value for the latest registration field.");

		public static INotification CategoryDoesNotExist(Settings instance)
		{
			return new ValidationWarning(String.Format("The category '{0}' does not exist.",
			                                           instance.CategoryName));
		}
	}
}