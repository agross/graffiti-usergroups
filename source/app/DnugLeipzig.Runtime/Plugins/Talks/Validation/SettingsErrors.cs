using System;

using DnugLeipzig.Definitions.Validation;
using DnugLeipzig.Runtime.Validation;

namespace DnugLeipzig.Runtime.Plugins.Talks.Validation
{
	public static class SettingsErrors
	{
		// TODO: English
		public static readonly INotification CategoryIsMissing = new ValidationError("Please enter a category name.");

		public static readonly INotification YearQueryStringIsMissing =
			new ValidationError("Please enter a year query string parameter.");

		public static INotification CategoryDoesNotExist(Settings instance)
		{
			return new ValidationWarning(String.Format("The category '{0}' does not exist.",
			                                           instance.CategoryName));
		}
	}
}