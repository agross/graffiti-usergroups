using System;
using System.Collections.Generic;

using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Runtime.Repositories;

using Graffiti.Core;

namespace DnugLeipzig.Plugins.Migration
{
	internal class FieldMigrator
	{
		readonly ICategoryRepository _categoryRepository;
		readonly IPostRepository _postRepository;

		#region Ctors
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
		internal void EnsureFields(string categoryName, Dictionary<string, FieldType> fields)
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
				KeyValuePair<string, FieldType> pair = field;
				if (formSettings.Fields.Exists(f => Graffiti.Core.Util.AreEqualIgnoreCase(pair.Key, f.Name)))
				{
					continue;
				}

				CustomField newField = new CustomField
				                       { Name = field.Key, Enabled = true, Id = Guid.NewGuid(), FieldType = field.Value };

				_categoryRepository.AddField(formSettings, newField);
			}

			// Order the fields according the order of the fields in the "fields" variable.
			string[] fieldNames = new string[fields.Keys.Count];
			fields.Keys.CopyTo(fieldNames, 0);
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
		                                Dictionary<string, string> changedFieldNames)
		{
			if (changedFieldNames == null)
			{
				// Nothing to do.
				return;
			}

			if (String.Equals(sourceCategoryName, targetCategoryName, StringComparison.OrdinalIgnoreCase) && changedFieldNames.Count == 0)
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