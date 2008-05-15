using Graffiti.Core;

namespace DnugLeipzig.Definitions.Repositories
{
	public interface ICategoryRepository : IRepository<Category>
	{
		Category GetCategory(string categoryName);
		void AddCategory(Category category);
		CustomFormSettings GetFormSettings(string categoryName);
		void DeleteCategory(string categoryName);
		bool IsExistingCategory(string categoryName);
		void AddField(CustomFormSettings settings, CustomField field);
		void DeleteField(CustomFormSettings settings, string fieldName);
		void SaveFormSettings(CustomFormSettings settings);
	}
}