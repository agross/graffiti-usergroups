using System;

using Graffiti.Core;

namespace DnugLeipzig.Plugins.Repositories
{
	public interface ICategoryRepository
	{
		Category GetCategory(string categoryName);
		void AddCategory(Category category);
		CustomFormSettings GetFormSettings(string categoryName);
		void AddField(CustomFormSettings settings, CustomField field);
		void DeleteCategory(string categoryName);
		void DeleteField(CustomFormSettings settings, string fieldName);
	}
}