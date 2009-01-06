using System;

using DnugLeipzig.Definitions.Validation;
using DnugLeipzig.Runtime.Validation;

namespace DnugLeipzig.Runtime.Plugins.Talks
{
	public static class TalkPluginSettingsErrors
	{
		// TODO: English
		public static readonly INotification CategoryIsMissing = new ValidationError("Please enter a category name.");

		public static readonly INotification YearQueryStringIsMissing =
			new ValidationError("Please enter a year query string parameter.");

		public static INotification CategoryDoesNotExist(TalkPluginSettings instance)
		{
			return new ValidationWarning(String.Format("The category '{0}' does not exist.",
			                                           instance.CategoryName));
		}
	}
}