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
			If(x => x[config.StartDateField].HasValue() && !x[config.StartDateField].IsDate())
				.AddNotification(EventErrors.InvalidDate(config.StartDateField));

			If(x => x[config.EndDateField].HasValue() && !x[config.EndDateField].IsDate())
				.AddNotification(EventErrors.InvalidDate(config.EndDateField));

			If(x => x[config.EndDateField].HasValue() && !x[config.StartDateField].HasValue())
				.AddNotification(EventErrors.StartDateIsRequiredWhenEndDateIsSet(config.StartDateField));

			If(x => x[config.StartDateField].IsDate() && x[config.EndDateField].IsDate() &&
			        x[config.StartDateField].ToDate() > x[config.EndDateField].ToDate())
				.AddNotification(EventErrors.StartDateMustBeBeforeEndDate(config.StartDateField, config.EndDateField));

			If(x => x[config.MaximumNumberOfRegistrationsField].HasValue() &&
			        !x[config.MaximumNumberOfRegistrationsField].ToInt(int.MinValue).IsInRange(0, int.MaxValue))
				.AddNotification(
				EventErrors.MaximumNumberOfRegistrationsMustBeEqualOrGreaterThanZero(config.MaximumNumberOfRegistrationsField));

			If(x => x[config.RegistrationRecipientField].HasValue() &&
			        !x[config.RegistrationRecipientField].IsEmail())
				.AddNotification(EventErrors.InvalidRegistrationRecipientEmail(config.RegistrationRecipientField));
		}
	}
}