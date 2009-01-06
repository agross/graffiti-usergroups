using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Runtime.Validation;

namespace DnugLeipzig.Runtime.Plugins.Events
{
	public class EventPluginSettingsValidator : Validator<EventPluginSettings>
	{
		readonly ICategoryRepository _categoryRepository;

		public EventPluginSettingsValidator(ICategoryRepository categoryRepository)
		{
			_categoryRepository = categoryRepository;

			IfNot(x => x.CategoryName.Exists())
				.AddNotification(EventPluginSettingsErrors.CategoryIsMissing);

			If(x => !x.CreateTargetCategoryAndFields &&
			        !_categoryRepository.Exists((x.CategoryName)))
				.AddNotification(EventPluginSettingsErrors.CategoryDoesNotExist);

			If(x => x.DefaultRegistrationRecipient.Exists() &&
			        !x.DefaultRegistrationRecipient.IsEmail())
				.AddNotification(EventPluginSettingsErrors.DefaultRegistrationRecipientEmailIsInvalid);

			If(x => x.DefaultMaximumNumberOfRegistrations.Exists() &&
			        !x.DefaultMaximumNumberOfRegistrations.IsInt())
				.AddNotification(EventPluginSettingsErrors.DefaultMaximumNumberOfRegistrationsMustBeAnInteger);

			If(x => x.DefaultMaximumNumberOfRegistrations.Exists() &&
			        x.DefaultMaximumNumberOfRegistrations.IsInt() &&
			        !int.Parse(x.DefaultMaximumNumberOfRegistrations).IsInRange(0, int.MaxValue))
				.AddNotification(EventPluginSettingsErrors.DefaultMaximumNumberOfRegistrationsMustBeEqualOrGreaterThanZero);

			IfNot(x => x.YearQueryString.Exists())
				.AddNotification(EventPluginSettingsErrors.YearQueryStringIsMissing);
		}
	}
}