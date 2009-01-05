using System;
using System.Collections.Generic;

using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Runtime.Plugins.Migration;
using DnugLeipzig.Runtime.Repositories;

using Graffiti.Core;

using MbUnit.Framework;

using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace DnugLeipzig.Runtime.Tests.Plugins.Migration
{
	public partial class MigratorTests
	{
		[Test]
		public void CreatesNewFields()
		{
			ICategoryRepository categoryRepository;
			const string categoryName = "category";

			List<FieldInfo> newFields = new List<FieldInfo>();
			newFields.Add(new FieldInfo("newField1", FieldType.TextBox, "New field 1"));
			newFields.Add(new FieldInfo("newField2", FieldType.CheckBox, "New field 2"));

			using (Mocks.Record())
			{
				categoryRepository = Mocks.StrictMock<ICategoryRepository>();

				CustomFormSettings formSettings = new CustomFormSettings();
				formSettings.Fields = new List<CustomField>();
				Expect.Call(categoryRepository.GetFormSettings(categoryName)).Return(formSettings);

				foreach (FieldInfo field in newFields)
				{
					categoryRepository.AddField(formSettings, new CustomField());

					FieldInfo field1 = field;
					LastCall.Constraints(Is.Same(formSettings),
					                     Is.Matching(
					                     	(CustomField f) =>
					                     	f.Name == field1.FieldName && f.FieldType == field1.FieldType &&
					                     	f.Description == field1.Description));
				}
			}

			using (Mocks.Playback())
			{
				Migrator fm = new Migrator(categoryRepository, new PostRepository());
				fm.EnsureFields(categoryName, newFields);
			}
		}

		[Test]
		public void DoesNotRecreateExistingFields()
		{
			ICategoryRepository categoryRepository;
			const string categoryName = "category";

			List<FieldInfo> existingFields = new List<FieldInfo>();
			existingFields.Add(new FieldInfo("existingField1", FieldType.TextBox, "Existing field 1"));
			existingFields.Add(new FieldInfo("existingField2", FieldType.CheckBox, "Existing field 2"));

			using (Mocks.Record())
			{
				categoryRepository = Mocks.StrictMock<ICategoryRepository>();

				CustomFormSettings formSettings = new CustomFormSettings();
				formSettings.Fields = existingFields.ToCustomFieldList();

				Expect.Call(categoryRepository.GetFormSettings(categoryName)).Return(formSettings);
			}

			using (Mocks.Playback())
			{
				Migrator fm = new Migrator(categoryRepository, new PostRepository());
				fm.EnsureFields(categoryName, existingFields);
			}
		}

		[Test]
		public void ShouldCreateNewFieldsAndLeaveExistingFieldsIntact()
		{
			ICategoryRepository categoryRepository;
			const string categoryName = "category";

			List<FieldInfo> existingFields = new List<FieldInfo>();
			existingFields.Add(new FieldInfo("existingField1", FieldType.TextBox, "Existing field 1"));
			existingFields.Add(new FieldInfo("existingField2", FieldType.CheckBox, "Existing field 2"));

			List<FieldInfo> newFields = new List<FieldInfo>();
			newFields.Add(new FieldInfo("newField1", FieldType.TextBox, "New field 1"));
			newFields.Add(new FieldInfo("newField2", FieldType.CheckBox, "New field 2"));

			using (Mocks.Record())
			{
				categoryRepository = Mocks.StrictMock<ICategoryRepository>();

				CustomFormSettings formSettings = new CustomFormSettings();
				formSettings.Fields = existingFields.ToCustomFieldList();
				Expect.Call(categoryRepository.GetFormSettings(categoryName)).Return(formSettings);

				foreach (FieldInfo field in newFields)
				{
					categoryRepository.AddField(null, null);

					FieldInfo field1 = field;
					LastCall.Constraints(Is.Same(formSettings),
					                     Is.Matching(
					                     	(CustomField f) =>
					                     	f.Name == field1.FieldName && f.FieldType == field1.FieldType &&
					                     	f.Description == field1.Description));
				}
			}

			using (Mocks.Playback())
			{
				List<FieldInfo> merged = new List<FieldInfo>();
				merged.AddRange(newFields);
				merged.AddRange(existingFields);

				Migrator fm = new Migrator(categoryRepository, new PostRepository());
				fm.EnsureFields(categoryName, merged);
			}
		}

		[Test]
		public void ShouldSetDescriptionForExistingFieldsIfEmpty()
		{
			ICategoryRepository categoryRepository;
			const string categoryName = "category";

			List<FieldInfo> existingFields = new List<FieldInfo>();
			existingFields.Add(new FieldInfo("existingField1", FieldType.TextBox, "User-defined description"));
			existingFields.Add(new FieldInfo("existingField2", FieldType.CheckBox, String.Empty));

			List<FieldInfo> newFields = new List<FieldInfo>();
			newFields.Add(new FieldInfo("existingField1", FieldType.TextBox, "Field description 1"));
			newFields.Add(new FieldInfo("existingField2", FieldType.CheckBox, "Field description 2"));

			using (Mocks.Record())
			{
				categoryRepository = Mocks.StrictMock<ICategoryRepository>();

				CustomFormSettings formSettings = new CustomFormSettings();
				formSettings.Fields = existingFields.ToCustomFieldList();
				Expect.Call(categoryRepository.GetFormSettings(categoryName)).Return(formSettings);

				List<CustomField> expectedFormSettings = new List<CustomField>();
				expectedFormSettings.Add(existingFields.ToCustomFieldList()[0]);
				expectedFormSettings.Add(newFields.ToCustomFieldList()[1]);

				categoryRepository.SaveFormSettings(null);
				LastCall.Constraints(Is.Matching((CustomFormSettings cfs) =>
					{
						for (int i = 0; i < cfs.Fields.Count; i++)
						{
							CustomField field = cfs.Fields[i];
							CustomField expectedField = expectedFormSettings[i];
							if (expectedField.Name != field.Name || expectedField.Description != field.Description ||
							    expectedField.FieldType != field.FieldType)
							{
								return false;
							}
						}

						return true;
					}));
			}

			using (Mocks.Playback())
			{
				Migrator fm = new Migrator(categoryRepository, new PostRepository());
				fm.EnsureFields(categoryName, newFields);
			}
		}

		[Test]
		public void ShouldDeleteChangedFields()
		{
			ICategoryRepository categoryRepository;
			IPostRepository postRepository;
			IMemento newState;
			IMemento oldState;
			const string categoryName = "category";

			using (Mocks.Record())
			{
				oldState = Mocks.StrictMock<IMemento>();
				Expect.Call(oldState.CategoryName).Return(categoryName);
				Expect.Call(oldState.Fields).Repeat.AtLeastOnce().Return(new Dictionary<Guid, FieldInfo>
				                                                         {
				                                                         	{
				                                                         		new Guid("{28E2469B-E568-4406-832D-3AD3F1EBE214}"),
				                                                         		new FieldInfo("oldFieldName",
				                                                         		              FieldType.TextBox,
				                                                         		              "Description")
				                                                         		}
				                                                         });

				newState = Mocks.StrictMock<IMemento>();
				Expect.Call(newState.CategoryName).Return(categoryName);
				Expect.Call(newState.Fields).Repeat.AtLeastOnce().Return(new Dictionary<Guid, FieldInfo>
				                                                         {
				                                                         	{
				                                                         		new Guid("{28E2469B-E568-4406-832D-3AD3F1EBE214}"),
				                                                         		new FieldInfo("newFieldName",
				                                                         		              FieldType.TextBox,
				                                                         		              "Description")
				                                                         		}
				                                                         });

				categoryRepository = Mocks.StrictMock<ICategoryRepository>();
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
				postRepository = Mocks.StrictMock<IPostRepository>();
				Expect.Call(postRepository.GetByCategory(categoryName)).Return(new PostCollection());
			}

			using (Mocks.Playback())
			{
				Migrator fm = new Migrator(categoryRepository, postRepository);
				fm.Migrate(new MigrationInfo(oldState, newState));
			}
		}
	}
}