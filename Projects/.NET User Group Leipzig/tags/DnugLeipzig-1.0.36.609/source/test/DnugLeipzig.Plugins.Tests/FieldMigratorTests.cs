using System.Collections.Generic;

using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Plugins.Migration;
using DnugLeipzig.Plugins.Tests.Extensions;
using DnugLeipzig.Runtime.Repositories;

using Graffiti.Core;

using MbUnit.Framework;

using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace DnugLeipzig.Plugins.Tests
{
	[TestFixture]
	public class FieldMigratorTests
	{
		readonly MockRepository _mocks = new MockRepository();

		[TearDown]
		public void TearDown()
		{
			_mocks.ReplayAll();
			_mocks.VerifyAll();
		}

		#region Category Creation
		[Test]
		public void CreatesNewCategory()
		{
			ICategoryRepository categoryRepository;
			const string categoryName = "new";

			using (_mocks.Record())
			{
				categoryRepository = _mocks.CreateMock<ICategoryRepository>();
				Expect.Call(categoryRepository.GetCategory(categoryName)).Return(null);

				categoryRepository.AddCategory(null);
				LastCall.Constraints(Is.Matching((Category category) => category.Name == categoryName));
			}

			using (_mocks.Playback())
			{
				FieldMigrator fm = new FieldMigrator(categoryRepository, new PostRepository());
				fm.EnsureTargetCategory(categoryName);
			}
		}

		[Test]
		public void DoesNotRecreateExistingCategory()
		{
			ICategoryRepository categoryRepository;
			const string categoryName = "existing";

			using (_mocks.Record())
			{
				categoryRepository = _mocks.CreateMock<ICategoryRepository>();
				Expect.Call(categoryRepository.GetCategory(categoryName)).Return(new Category());
			}

			using (_mocks.Playback())
			{
				FieldMigrator fm = new FieldMigrator(categoryRepository, new PostRepository());
				fm.EnsureTargetCategory(categoryName);
			}
		}

		[Test]
		public void CreatesNewCategoryAndDeletesOldCategoryIfCategoryNameChanges()
		{
			ICategoryRepository categoryRepository;
			IPostRepository postRepository;
			IMemento newState;
			IMemento oldState;
			const string sourceCategoryName = "old";
			const string targetCategoryName = "new";

			using (_mocks.Record())
			{
				oldState = _mocks.CreateMock<IMemento>();
				Expect.Call(oldState.CategoryName).Return(sourceCategoryName);
				Expect.Call(oldState.Fields).Return(new Dictionary<string, FieldInfo>());

				newState = _mocks.CreateMock<IMemento>();
				Expect.Call(newState.CategoryName).Return(targetCategoryName);
				Expect.Call(newState.Fields).Return(new Dictionary<string, FieldInfo>());

				categoryRepository = _mocks.CreateMock<ICategoryRepository>();
				Expect.Call(categoryRepository.GetCategory(targetCategoryName)).Return(null);

				Category targetCategory = new Category { Id = int.MaxValue, Name = targetCategoryName };
				Expect.Call(categoryRepository.GetCategory(targetCategoryName)).Return(targetCategory);

				categoryRepository.AddCategory(null);
				LastCall.Constraints(Is.Matching((Category category) => category.Name == targetCategoryName));

				categoryRepository.DeleteCategory(null);
				LastCall.Constraints(Is.Equal(sourceCategoryName));

				// No posts to migrate.
				postRepository = _mocks.CreateMock<IPostRepository>();
				Expect.Call(postRepository.GetByCategory(sourceCategoryName)).Return(new PostCollection());
			}

			using (_mocks.Playback())
			{
				FieldMigrator fm = new FieldMigrator(categoryRepository, postRepository);
				fm.Migrate(new MigrationInfo(oldState, newState));
			}
		}
		#endregion

		#region Custom Field Creation
		[Test]
		public void CreatesNewFields()
		{
			ICategoryRepository categoryRepository;
			const string categoryName = "category";

			List<CustomField> newFields = new List<CustomField>();
			newFields.Add(new CustomField { Name = "newField1", FieldType = FieldType.TextBox });
			newFields.Add(new CustomField { Name = "newField2", FieldType = FieldType.CheckBox });

			using (_mocks.Record())
			{
				categoryRepository = _mocks.CreateMock<ICategoryRepository>();

				CustomFormSettings formSettings = new CustomFormSettings();
				formSettings.Fields = new List<CustomField>();
				Expect.Call(categoryRepository.GetFormSettings(categoryName)).Return(formSettings);

				foreach (CustomField field in newFields)
				{
					categoryRepository.AddField(formSettings, new CustomField());

					CustomField field1 = field;
					LastCall.Constraints(Is.Same(formSettings),
					                     Is.Matching((CustomField f) => f.Name == field1.Name && f.FieldType == field1.FieldType));
				}
			}

			using (_mocks.Playback())
			{
				FieldMigrator fm = new FieldMigrator(categoryRepository, new PostRepository());
				fm.EnsureFields(categoryName, newFields.ToCustomFieldDictionary());
			}
		}

		[Test]
		public void DoesNotRecreateExistingFields()
		{
			ICategoryRepository categoryRepository;
			const string categoryName = "category";

			List<CustomField> existingFields = new List<CustomField>();
			existingFields.Add(new CustomField { Name = "existingField1", FieldType = FieldType.TextBox });
			existingFields.Add(new CustomField { Name = "existingField2", FieldType = FieldType.CheckBox });

			using (_mocks.Record())
			{
				categoryRepository = _mocks.CreateMock<ICategoryRepository>();

				CustomFormSettings formSettings = new CustomFormSettings();
				formSettings.Fields = existingFields;

				Expect.Call(categoryRepository.GetFormSettings(categoryName)).Return(formSettings);
			}

			using (_mocks.Playback())
			{
				FieldMigrator fm = new FieldMigrator(categoryRepository, new PostRepository());
				fm.EnsureFields(categoryName, existingFields.ToCustomFieldDictionary());
			}
		}

		[Test]
		public void CreatesNewFieldsAndLeavesExistingFieldsIntact()
		{
			ICategoryRepository categoryRepository;
			const string categoryName = "category";

			List<CustomField> existingFields = new List<CustomField>();
			existingFields.Add(new CustomField { Name = "existingField1", FieldType = FieldType.TextBox });
			existingFields.Add(new CustomField { Name = "existingField2", FieldType = FieldType.CheckBox });

			List<CustomField> newFields = new List<CustomField>();
			newFields.Add(new CustomField { Name = "newField1", FieldType = FieldType.TextBox });
			newFields.Add(new CustomField { Name = "newField2", FieldType = FieldType.CheckBox });

			using (_mocks.Record())
			{
				categoryRepository = _mocks.CreateMock<ICategoryRepository>();

				CustomFormSettings formSettings = new CustomFormSettings();
				formSettings.Fields = existingFields;
				Expect.Call(categoryRepository.GetFormSettings(categoryName)).Return(formSettings);

				foreach (CustomField field in newFields)
				{
					categoryRepository.AddField(null, null);

					CustomField field1 = field;
					LastCall.Constraints(Is.Same(formSettings),
					                     Is.Matching((CustomField f) => f.Name == field1.Name && f.FieldType == field1.FieldType));
				}
			}

			using (_mocks.Playback())
			{
				// Merge the two dictionaries.
				Dictionary<string, FieldType> fields = existingFields.ToCustomFieldDictionary();
				fields.AddRange(newFields.ToCustomFieldDictionary());

				FieldMigrator fm = new FieldMigrator(categoryRepository, new PostRepository());
				fm.EnsureFields(categoryName, fields);
			}
		}

		[Test]
		public void DeletesChangedFields()
		{
			ICategoryRepository categoryRepository;
			IPostRepository postRepository;
			IMemento newState;
			IMemento oldState;
			const string categoryName = "category";

			using (_mocks.Record())
			{
				oldState = _mocks.CreateMock<IMemento>();
				Expect.Call(oldState.CategoryName).Return(categoryName);
				Expect.Call(oldState.Fields).Repeat.AtLeastOnce().Return(new Dictionary<string, FieldInfo>
				                                                         {
				                                                         	{
				                                                         		"oldFieldName",
				                                                         		new FieldInfo("oldFieldName", FieldType.TextBox)
				                                                         		}
				                                                         });

				newState = _mocks.CreateMock<IMemento>();
				Expect.Call(newState.CategoryName).Return(categoryName);
				Expect.Call(newState.Fields).Repeat.AtLeastOnce().Return(new Dictionary<string, FieldInfo>
				                                                         {
				                                                         	{
				                                                         		"oldFieldName",
				                                                         		new FieldInfo("newFieldName", FieldType.TextBox)
				                                                         		}
				                                                         });

				categoryRepository = _mocks.CreateMock<ICategoryRepository>();
				Category targetCategory = new Category { Id = int.MaxValue, Name = categoryName };
				Expect.Call(categoryRepository.GetCategory(categoryName)).Repeat.AtLeastOnce().Return(targetCategory);

				CustomFormSettings formSettings = new CustomFormSettings();
				formSettings.Fields = new List<CustomField>();
				Expect.Call(categoryRepository.GetFormSettings(categoryName)).Repeat.AtLeastOnce().Return(formSettings);

				categoryRepository.AddField(null, null);
				LastCall.Constraints(Is.Same(formSettings), Is.Matching((CustomField f) => f.Name == "newFieldName"));

				categoryRepository.DeleteField(null, null);
				LastCall.Constraints(Is.Same(formSettings), Is.Equal("oldFieldName"));

				// No posts to migrate.
				postRepository = _mocks.CreateMock<IPostRepository>();
				Expect.Call(postRepository.GetByCategory(categoryName)).Return(new PostCollection());
			}

			using (_mocks.Playback())
			{
				FieldMigrator fm = new FieldMigrator(categoryRepository, postRepository);
				fm.Migrate(new MigrationInfo(oldState, newState));
			}
		}
		#endregion
	}
}