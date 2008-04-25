using System;
using System.Collections.Generic;

using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Runtime.Repositories;

using Graffiti.Core;

namespace DnugLeipzig.Plugins.Migration
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

			Category category = CategoryRepository.GetCategory(categoryName);
			if (category != null)
			{
				return;
			}

			category = new Category { Name = categoryName, ParentId = -1 };
			CategoryRepository.AddCategory(category);
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

			if (fields == null)
			{
				throw new ArgumentNullException("fields");
			}

			if (fields.Count == 0)
			{
				// Nothing to to.
				return;
			}

			CustomFormSettings formSettings = CategoryRepository.GetFormSettings(categoryName);

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

				CategoryRepository.AddField(formSettings, newField);
			}
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

			CategoryRepository.DeleteCategory(categoryName);
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

			if (fieldNames.Count == 0)
			{
				// Nothing to do.
				return;
			}

			CustomFormSettings formSettings = CategoryRepository.GetFormSettings(categoryName);

			foreach (string field in fieldNames)
			{
				CategoryRepository.DeleteField(formSettings, field);
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
				throw new ArgumentNullException("changedFieldNames");
			}

			if (String.Equals(sourceCategoryName, targetCategoryName, StringComparison.OrdinalIgnoreCase) && changedFieldNames.Count == 0)
			{
				// Nothing to do.
				return;
			}

			IList<Post> posts = PostRepository.GetByCategory(sourceCategoryName);
			Category targetCategory = CategoryRepository.GetCategory(targetCategoryName);

			foreach (Post post in posts)
			{
				// Migrate post.
				post.CategoryId = targetCategory.Id;

				// Migrate field values.
				foreach (var fieldNames in changedFieldNames)
				{
					// Copy field value to the new field name.
					post.CustomFields().Add(fieldNames.Value, post.Custom(fieldNames.Key));

					// Delete old field value.
					post.CustomFields().Remove(fieldNames.Key);
				}

				PostRepository.Save(post);
			}
		}
	}
}