using DnugLeipzig.Definitions.Extensions;
using DnugLeipzig.Definitions.Plugins.Events;
using DnugLeipzig.Runtime.Validation;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Plugins.Events.Validation
{
	public class EventValidator : Validator<Post>, IEventValidator
	{
		public EventValidator(IEventPluginConfigurationProvider config)
		{
			RequireValidDateRangeWithOptionalValues(config.StartDateField, config.EndDateField);
			If(x => x[config.EndDateField].HasValue() && !x[config.StartDateField].HasValue())
				.AddNotification(EventErrors.StartDateIsRequiredWhenEndDateIsSet(config.StartDateField));

			RequireValidDateRangeWithOptionalValues(config.EarliestRegistrationField, config.LatestRegistrationField);

			If(x => x[config.MaximumNumberOfRegistrationsField].HasValue() &&
			        !x[config.MaximumNumberOfRegistrationsField].ToInt(int.MinValue).IsInRange(0, int.MaxValue))
				.AddNotification(
				EventErrors.MaximumNumberOfRegistrationsMustBeEqualOrGreaterThanZero(config.MaximumNumberOfRegistrationsField));

			If(x => x[config.RegistrationRecipientField].HasValue() &&
			        !x[config.RegistrationRecipientField].IsEmail())
				.AddNotification(EventErrors.InvalidRegistrationRecipientEmail(config.RegistrationRecipientField));
		}

		void RequireValidDateRangeWithOptionalValues(string startDateField, string endDateField)
		{
			If(x => x[startDateField].HasValue() && !x[startDateField].IsDate())
				.AddNotification(EventErrors.InvalidDate(startDateField));

			If(x => x[endDateField].HasValue() && !x[endDateField].IsDate())
				.AddNotification(EventErrors.InvalidDate(endDateField));

			If(x => x[startDateField].IsDate() && x[endDateField].IsDate() &&
			        x[startDateField].ToDate() > x[endDateField].ToDate())
				.AddNotification(EventErrors.StartDateMustBeBeforeEndDate(startDateField, endDateField));
		}
	}
}