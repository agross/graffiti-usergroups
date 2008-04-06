using System;
using System.Collections.Generic;

using DnugLeipzig.Plugins;
using DnugLeipzig.Plugins.Repositories;

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

		#region Categories
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
				fm.SetUpCategory(categoryName);
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
				fm.SetUpCategory(categoryName);
			}
		}
		#endregion

		#region Custom fields
		[Test]
		public void CreatesNewFields()
		{
			ICategoryRepository categoryRepository;
			const string categoryName = "category";

			Dictionary<string, FieldType> newFields = new Dictionary<string, FieldType>();
			newFields.Add("field1", FieldType.TextBox);
			newFields.Add("field2", FieldType.CheckBox);

			using (_mocks.Record())
			{
				categoryRepository = _mocks.CreateMock<ICategoryRepository>();

				CustomFormSettings formSettings = new CustomFormSettings();
				formSettings.Fields = new List<CustomField>();
				Expect.Call(categoryRepository.GetFormSettings(categoryName)).Return(formSettings);

				foreach (KeyValuePair<string, FieldType> pair in newFields)
				{
					KeyValuePair<string, FieldType> kvp = pair;

					categoryRepository.AddField(formSettings, new CustomField());
					LastCall.Constraints(Is.Same(formSettings),
										 Is.Matching((CustomField field) => field.Name == kvp.Key && field.FieldType == kvp.Value));	
				}
			}

			using (_mocks.Playback())
			{
				FieldMigrator fm = new FieldMigrator(categoryRepository, new PostRepository());
				fm.SetUpFields(categoryName, newFields);
			}
		}

		[Test]
		public void DoesNotRecreateExistingFields()
		{
			ICategoryRepository categoryRepository;
			const string categoryName = "category";

			Dictionary<string, FieldType> existingFields = new Dictionary<string, FieldType>();
			existingFields.Add("field1", FieldType.TextBox);
			existingFields.Add("field2", FieldType.CheckBox);

			using (_mocks.Record())
			{
				categoryRepository = _mocks.CreateMock<ICategoryRepository>();

				CustomFormSettings formSettings = new CustomFormSettings();
				formSettings.Fields = new List<CustomField>();
				foreach (KeyValuePair<string, FieldType> pair in existingFields)
				{
					formSettings.Fields.Add(new CustomField { Name = pair.Key});
				}

				Expect.Call(categoryRepository.GetFormSettings(categoryName)).Return(formSettings);
			}

			using (_mocks.Playback())
			{
				FieldMigrator fm = new FieldMigrator(categoryRepository, new PostRepository());
				fm.SetUpFields(categoryName, existingFields);
			}
		}
		
		[Test]
		public void CreatesNewFieldsWithExistingFields()
		{
			ICategoryRepository categoryRepository;
			const string categoryName = "category";

			Dictionary<string, FieldType> existingFields = new Dictionary<string, FieldType>();
			existingFields.Add("existingField1", FieldType.TextBox);
			existingFields.Add("existingField2", FieldType.CheckBox);

			Dictionary<string, FieldType> newFields = new Dictionary<string, FieldType>();
			existingFields.Add("newField1", FieldType.TextBox);
			existingFields.Add("newField2", FieldType.CheckBox);

			using (_mocks.Record())
			{
				categoryRepository = _mocks.CreateMock<ICategoryRepository>();

				CustomFormSettings formSettings = new CustomFormSettings();
				formSettings.Fields = new List<CustomField>();
				foreach (KeyValuePair<string, FieldType> pair in existingFields)
				{
					formSettings.Fields.Add(new CustomField { Name = pair.Key});
				}

				Expect.Call(categoryRepository.GetFormSettings(categoryName)).Return(formSettings);

				foreach (KeyValuePair<string, FieldType> pair in newFields)
				{
					KeyValuePair<string, FieldType> kvp = pair;

					categoryRepository.AddField(formSettings, new CustomField());
					LastCall.Constraints(Is.Same(formSettings),
										 Is.Matching((CustomField field) => field.Name == kvp.Key && field.FieldType == kvp.Value));
				}
			}

			using (_mocks.Playback())
			{
				FieldMigrator fm = new FieldMigrator(categoryRepository, new PostRepository());
				fm.SetUpFields(categoryName, existingFields);
			}
		}
		#endregion

	}
}