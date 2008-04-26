using Graffiti.Core;

namespace DnugLeipzig.Definitions.Repositories
{
	public interface ICategoryRepository : IRepository<Category>
	{
		Category GetCategory(string categoryName);
		void AddCategory(Category category);
		CustomFormSettings GetFormSettings(string categoryName);
		void AddField(CustomFormSettings settings, CustomField field);
		void DeleteCategory(string categoryName);
		void DeleteField(CustomFormSettings settings, string fieldName);
		bool IsExistingCategory(string categoryName);
	}
}