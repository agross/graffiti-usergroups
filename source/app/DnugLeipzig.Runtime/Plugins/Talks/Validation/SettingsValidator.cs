using DnugLeipzig.Definitions.Extensions;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Runtime.Validation;

namespace DnugLeipzig.Runtime.Plugins.Talks.Validation
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
			        !_categoryRepository.Exists(x.CategoryName))
				.AddNotification(SettingsErrors.CategoryDoesNotExist);

			IfNot(x => x.YearQueryString.HasValue())
				.AddNotification(SettingsErrors.YearQueryStringIsMissing);
			
			IfNot(x => x.Date.HasValue())
				.AddNotification(SettingsErrors.DateFieldIsMissing);
	
			IfNot(x => x.Speaker.HasValue())
				.AddNotification(SettingsErrors.SpeakerFieldIsMissing);
		}
	}
}