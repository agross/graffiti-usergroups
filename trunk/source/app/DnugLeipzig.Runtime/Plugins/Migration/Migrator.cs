using System;
using System.Collections.Generic;

using DnugLeipzig.Definitions.Extensions;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Runtime.Repositories;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Plugins.Migration
{
	internal class Migrator
	{
		readonly ICategoryRepository _categoryRepository;
		readonly IPostRepository _postRepository;

		#region Ctors
		public Migrator() : this(new CategoryRepository(), new PostRepository())
		{
		}

		public Migrator(ICategoryRepository categoryRepository, IPostRepository postRepository)
		{
			if (categoryRepository == null)
			{
				throw new ArgumentNullException("categoryRepository");
			}

			if (postRepository == null)
			{
				throw new ArgumentNullException("postRepository");
			}

			_categoryRepository = categoryRepository;
			_postRepository = postRepository;
		}
		#endregion

		public void Migrate(MigrationInfo migrationInfo)
		{
			if (migrationInfo == null)
			{
				throw new ArgumentNullException("migrationInfo");
			}

			EnsureTargetCategory(migrationInfo.TargetCategoryName);
			EnsureFields(migrationInfo.TargetCategoryName, migrationInfo.AllFields);

			MigratePostsAndFieldValues(migrationInfo.SourceCategoryName,
			                           migrationInfo.TargetCategoryName,
			                           migrationInfo.ChangedFieldNames);

			// Delete old fields (names are the ChangedFieldNames.Keys collection).
			string[] fieldsToDelete = new string[migrationInfo.ChangedFieldNames.Keys.Count];
			migrationInfo.ChangedFieldNames.Keys.CopyTo(fieldsToDelete, 0);
			DeleteFields(migrationInfo.TargetCategoryName, fieldsToDelete);

			if (!String.Equals(migrationInfo.SourceCategoryName, migrationInfo.TargetCategoryName))
			{
				DeleteCategory(migrationInfo.SourceCategoryName);
			}
		}

		/// <summary>
		/// Creates a new top-level category if it does not exist.
		/// </summary>
		/// <param name="categoryName">Name of the category.</param>
		internal void EnsureTargetCategory(string categoryName)
		{
			if (String.IsNullOrEmpty(categoryName))
			{
				throw new ArgumentOutOfRangeException("categoryName");
			}

			Category category = _categoryRepository.GetCategory(categoryName);
			if (category != null)
			{
				return;
			}

			category = new Category { Name = categoryName, ParentId = -1 };
			_categoryRepository.AddCategory(category);
		}

		/// <summary>
		/// Creates fields in a category if they do not exist. Does not change any existing fields.
		/// </summary>
		/// <param name="categoryName">Name of the category.</param>
		/// <param name="fields">The fields.</param>
		internal void EnsureFields(string categoryName, List<FieldInfo> fields)
		{
			if (String.IsNullOrEmpty(categoryName))
			{
				throw new ArgumentOutOfRangeException("categoryName");
			}

			if (fields == null || fields.Count == 0)
			{
				// Nothing to to.
				return;
			}

			CustomFormSettings formSettings = _categoryRepository.GetFormSettings(categoryName);

			// Ensure that the fields exist.
			foreach (var field in fields)
			{
				FieldInfo field1 = field;
				CustomField customField =
					formSettings.Fields.Find(f => Util.AreEqualIgnoreCase(field1.FieldName, f.Name));

				if (customField != null)
				{
					EnsureFieldDescription(field, formSettings);
					continue;
				}

				CreateField(field, formSettings);
			}

			SortFields(fields, formSettings);
		}

		void EnsureFieldDescription(FieldInfo field, CustomFormSettings formSettings)
		{
			CustomField customField =
				formSettings.Fields.Find(f => Util.AreEqualIgnoreCase(field.FieldName, f.Name));

			if (!customField.Description.IsNullOrEmpty())
			{
				return;
			}

			if (String.Equals(customField.Description, field.Description, StringComparison.Ordinal))
			{
				return;
			}

			customField.Description = field.Description;
			_categoryRepository.SaveFormSettings(formSettings);
		}

		void CreateField(FieldInfo field, CustomFormSettings formSettings)
		{
			CustomField newField = new CustomField
			                       {
			                       	Id = Guid.NewGuid(),
			                       	Enabled = true,
			                       	Name = field.FieldName,
			                       	FieldType = field.FieldType,
			                       	Description = field.Description
			                       };

			_categoryRepository.AddField(formSettings, newField);
		}

		/// <summary>
		/// Sorts the fields according the order of the fields in the <paramref name="fields"/> list.
		/// </summary>
		/// <param name="fields">The fields.</param>
		/// <param name="formSettings">The form settings.</param>
		void SortFields(List<FieldInfo> fields, CustomFormSettings formSettings)
		{
			string[] fieldNames = fields.ConvertAll(field => field.FieldName).ToArray();

			formSettings.Fields.Sort(delegate(CustomField x, CustomField y)
				{
					if (Array.IndexOf(fieldNames, x.Name) < Array.IndexOf(fieldNames, y.Name))
					{
						return -1;
					}

					if (Array.IndexOf(fieldNames, x.Name) > Array.IndexOf(fieldNames, y.Name))
					{
						return 1;
					}

					return 0;
				});
		}

		/// <summary>
		/// Deletes the category.
		/// </summary>
		/// <param name="categoryName">Name of the category.</param>
		void DeleteCategory(string categoryName)
		{
			if (String.IsNullOrEmpty(categoryName))
			{
				throw new ArgumentOutOfRangeException("categoryName");
			}

			_categoryRepository.DeleteCategory(categoryName);
		}

		/// <summary>
		/// Deletes fields in a category.
		/// </summary>
		/// <param name="categoryName">Name of the category.</param>
		/// <param name="fieldNames">The fields to delete.</param>
		void DeleteFields(string categoryName, ICollection<string> fieldNames)
		{
			if (String.IsNullOrEmpty(categoryName))
			{
				throw new ArgumentOutOfRangeException("categoryName");
			}

			if (fieldNames == null || fieldNames.Count == 0)
			{
				// Nothing to do.
				return;
			}

			CustomFormSettings formSettings = _categoryRepository.GetFormSettings(categoryName);

			foreach (string field in fieldNames)
			{
				_categoryRepository.DeleteField(formSettings, field);
			}
		}

		/// <summary>
		/// Migrates posts to the target category. Also migrates field values to the new fields.
		/// </summary>
		void MigratePostsAndFieldValues(string sourceCategoryName,
		                                string targetCategoryName,
		                                ICollection<KeyValuePair<string, string>> changedFieldNames)
		{
			if (changedFieldNames == null)
			{
				// Nothing to do.
				return;
			}

			if (String.Equals(sourceCategoryName, targetCategoryName, StringComparison.OrdinalIgnoreCase) &&
			    changedFieldNames.Count == 0)
			{
				// Nothing to do.
				return;
			}

			IList<Post> posts = _postRepository.GetByCategory(sourceCategoryName);
			Category targetCategory = _categoryRepository.GetCategory(targetCategoryName);

			foreach (Post post in posts)
			{
				// Migrate post.
				post.CategoryId = targetCategory.Id;

				// Migrate field values.
				foreach (var fieldNames in changedFieldNames)
				{
					// Copy field value to the new field name.
					post.CustomFields().Add(fieldNames.Value, post[fieldNames.Key]);

					// Delete old field value.
					post.CustomFields().Remove(fieldNames.Key);
				}

				_postRepository.Save(post);
			}
		}
	}
}