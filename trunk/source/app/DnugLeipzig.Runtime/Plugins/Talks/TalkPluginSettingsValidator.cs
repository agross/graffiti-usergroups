using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Runtime.Validation;

namespace DnugLeipzig.Runtime.Plugins.Talks
{
	public class TalkPluginSettingsValidator : Validator<TalkPluginSettings>
	{
		readonly ICategoryRepository _categoryRepository;

		public TalkPluginSettingsValidator(ICategoryRepository categoryRepository)
		{
			_categoryRepository = categoryRepository;

			IfNot(x => x.CategoryName.Exists())
				.AddNotification(TalkPluginSettingsErrors.CategoryIsMissing);

			If(x => !x.CreateTargetCategoryAndFields &&
			        !_categoryRepository.Exists(x.CategoryName))
				.AddNotification(TalkPluginSettingsErrors.CategoryDoesNotExist);

			IfNot(x => x.YearQueryString.Exists())
				.AddNotification(TalkPluginSettingsErrors.YearQueryStringIsMissing);
		}
	}
}