using System;
using System.Collections.Generic;

using DnugLeipzig.Plugins.Migration;
using DnugLeipzig.Plugins.Repositories;

using Graffiti.Core;

namespace DnugLeipzig.Plugins
{
	internal class FieldMigrator
	{
		readonly ICategoryRepository CategoryRepository;
		readonly IPostRepository PostRepository;

		#region ctors
		public FieldMigrator() : this(new CategoryRepository(), new PostRepository())
		{
		}

		public FieldMigrator(ICategoryRepository categoryRepository, IPostRepository postRepository)
		{
			if (categoryRepository == null)
			{
				throw new ArgumentNullException("categoryRepository");
			}

			if (postRepository == null)
			{
				throw new ArgumentNullException("postRepository");
			}


			CategoryRepository = categoryRepository;
			PostRepository = postRepository;
		}
		#endregion

		public void SetUpCategory(string categoryName)
		{
			if (String.IsNullOrEmpty(categoryName))
			{
				throw new ArgumentOutOfRangeException("categoryName");
			}

			Category category = CategoryRepository.GetCategory(categoryName);
			if (category != null)
			{
				return;
			}

			category = new Category { Name = categoryName, ParentId = -1 };
			CategoryRepository.AddCategory(category);
		}

		public void SetUpFields(string categoryName, Dictionary<string, FieldType> fieldNames)
		{
			if (String.IsNullOrEmpty(categoryName))
			{
				throw new ArgumentOutOfRangeException("categoryName");
			}

			if (fieldNames == null)
			{
				throw new ArgumentNullException("fieldNames");
			}

			CustomFormSettings settings = CategoryRepository.GetFormSettings(categoryName);

			// Ensure that the destination fields exist.
			foreach (var fieldName in fieldNames)
			{
				KeyValuePair<string, FieldType> pair = fieldName;
				if (settings.Fields.Exists(field => Graffiti.Core.Util.AreEqualIgnoreCase(pair.Key, field.Name)))
				{
					continue;
				}

				CustomField newField = new CustomField
				                       { Name = fieldName.Key, Enabled = true, Id = Guid.NewGuid(), FieldType = fieldName.Value };

				CategoryRepository.AddField(settings, newField);
			}
		}

		void DeleteCategory(string categoryName)
		{
			if (String.IsNullOrEmpty(categoryName))
			{
				throw new ArgumentOutOfRangeException("categoryName");
			}

			CategoryRepository.DeleteCategory(categoryName);
		}

		void DeleteFields(string categoryName, Dictionary<string, string> fieldNames)
		{
			if (String.IsNullOrEmpty(categoryName))
			{
				throw new ArgumentOutOfRangeException("categoryName");
			}

			if (fieldNames == null)
			{
				throw new ArgumentNullException("fieldNames");
			}

			CustomFormSettings settings = CategoryRepository.GetFormSettings(categoryName);

			// Ensure that the destination fields exist.
			foreach (var fieldName in fieldNames)
			{
				CategoryRepository.DeleteField(settings, fieldName.Key);
			}
		}

		public void MigrateValues(MigrationInfo migrationInfo)
		{
			if (migrationInfo == null)
			{
				throw new ArgumentNullException("migrationInfo");
			}

			// Ensure new category and fields.
			SetUpCategory(migrationInfo.NewCategoryName);
			SetUpFields(migrationInfo.NewCategoryName, migrationInfo.FieldTypes);

			Category newCategory = CategoryRepository.GetCategory(migrationInfo.NewCategoryName);

			// Migrate custom fields.
			foreach (var fieldName in migrationInfo.ChangedFieldNames)
			{
				if (String.Equals(fieldName.Key, fieldName.Value, StringComparison.OrdinalIgnoreCase))
				{
					// Field name is unchanged.
					continue;
				}

				PostCollection posts = PostRepository.GetPosts(migrationInfo.OldCategoryName);
				foreach (Post post in posts)
				{
					// Copy old field value.
					post.CustomFields().Add(fieldName.Value, post.Custom(fieldName.Key));
					
					// Delete old field value.
					post.CustomFields().Remove(fieldName.Key);

					PostRepository.Save(post);
				}
			}

			DeleteFields(migrationInfo.NewCategoryName, migrationInfo.ChangedFieldNames);

			// Migrate posts to the new category.
			if (!String.Equals(migrationInfo.OldCategoryName, migrationInfo.NewCategoryName))
			{
				PostCollection posts = PostRepository.GetPosts(migrationInfo.OldCategoryName);
				foreach (Post post in posts)
				{
					post.CategoryId = newCategory.Id;
					PostRepository.Save(post);
				}

				// Delete old category and fields.
				DeleteCategory(migrationInfo.OldCategoryName);
				DeleteFields(migrationInfo.OldCategoryName, migrationInfo.ChangedFieldNames);
			}
		}
	}
}