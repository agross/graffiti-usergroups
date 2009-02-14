using DnugLeipzig.Definitions.Extensions;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Runtime.Validation;

namespace DnugLeipzig.Runtime.Plugins.Events.Validation
{
	public class SettingsValidator : Validator<Settings>
	{
		readonly ICategoryRepository _categoryRepository;

		public SettingsValidator(ICategoryRepository categoryRepository)
		{
			_categoryRepository = categoryRepository;

			IfNot(x => x.CategoryName.HasValue())
				.AddNotification(SettingsErrors.CategoryIsMissing);

			If(x => !x.CreateTargetCategoryAndFields &&
			        !_categoryRepository.Exists((x.CategoryName)))
				.AddNotification(SettingsErrors.CategoryDoesNotExist);

			If(x => x.DefaultRegistrationRecipient.HasValue() &&
			        !x.DefaultRegistrationRecipient.IsEmail())
				.AddNotification(SettingsErrors.InvalidDefaultRegistrationRecipientEmail);

			If(x => x.DefaultMaximumNumberOfRegistrations.HasValue() &&
			        !x.DefaultMaximumNumberOfRegistrations.ToInt(int.MinValue).IsInRange(0, int.MaxValue))
				.AddNotification(SettingsErrors.DefaultMaximumNumberOfRegistrationsMustEqualOrGreaterThanZero);

			IfNot(x => x.YearQueryString.HasValue())
				.AddNotification(SettingsErrors.YearQueryStringIsMissing);
		}
	}
}