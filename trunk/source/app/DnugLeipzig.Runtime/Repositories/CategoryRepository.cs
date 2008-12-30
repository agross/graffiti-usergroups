using System;

using DnugLeipzig.Definitions.Repositories;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Repositories
{
	public class CategoryRepository : Repository, ICategoryRepository
	{
		static readonly Data Data = new Data();

		#region ICategoryRepository Members
		public Category GetCategory(string categoryName)
		{
			return Data.GetCategory(categoryName);
		}

		public void AddCategory(Category category)
		{
			if (category == null)
			{
				throw new ArgumentNullException("category");
			}

			category.Save();
		}

		public CustomFormSettings GetFormSettings(string categoryName)
		{
			if (String.IsNullOrEmpty(categoryName))
			{
				throw new ArgumentOutOfRangeException("categoryName");
			}

			Category category = GetCategory(categoryName);
			return CustomFormSettings.Get(category);
		}

		public void DeleteCategory(string categoryName)
		{
			if (String.IsNullOrEmpty(categoryName))
			{
				throw new ArgumentOutOfRangeException("categoryName");
			}

			Category category = GetCategory(categoryName);

			Post.DestroyDeletedPostCascadingForCategory(category.Id);
			Category.Destroy(Category.Columns.Id, category.Id);
			NavigationSettings settings = NavigationSettings.Get();

			DynamicNavigationItem item = settings.Items.Find(dni => dni.CategoryId == category.Id);
			if (item != null)
			{
				NavigationSettings.Remove(item.Id);
			}
			CategoryController.Reset();
		}

		public bool IsExistingCategory(string categoryName)
		{
			return GraffitiData.GetCategory(categoryName) != null;
		}

		public Data GraffitiData
		{
			get { return Data; }
		}

		public Category GetById(int id)
		{
			throw new NotSupportedException();
		}

		public void Save(Category post)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Custom Fields
		public void AddField(CustomFormSettings settings, CustomField field)
		{
			if (settings == null)
			{
				throw new ArgumentNullException("settings");
			}

			if (field == null)
			{
				throw new ArgumentNullException("field");
			}

			settings.Add(field);
			settings.Save();
		}

		public void SaveFormSettings(CustomFormSettings settings)
		{
			if (settings == null)
			{
				throw new ArgumentNullException("settings");
			}

			settings.Save();
		}

		public void DeleteField(CustomFormSettings settings, string fieldName)
		{
			CustomField field = settings.Fields.Find(cf => cf.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
			if (field != null)
			{
				settings.Fields.Remove(field);
				settings.Save();
			}
		}
		#endregion
	}
}