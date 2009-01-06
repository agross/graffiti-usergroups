using System.Collections.Specialized;
using System.Linq;

using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Definitions.Validation;
using DnugLeipzig.ForTesting;
using DnugLeipzig.Runtime.Plugins.Talks;

using MbUnit.Framework;

using Rhino.Mocks;

namespace DnugLeipzig.Runtime.Tests.Plugins.Talks
{
	public class
		When_the_plugin_settings_are_validated_and_the_category_does_not_exists_and_it_should_not_create_the_category
		: Spec
	{
		ValidationReport _report;
		TalkPluginSettings _settings;
		IValidator<TalkPluginSettings> _sut;

		protected override void Establish_context()
		{
			var categoryRepository = MockRepository.GenerateMock<ICategoryRepository>();
			_sut = new TalkPluginSettingsValidator(categoryRepository);

			categoryRepository.Stub(x => x.Exists(null))
				.IgnoreArguments()
				.Return(false);

			_settings = new TalkPluginSettings
			            {
			            	CategoryName = "talk category",
			            	YearQueryString = "year query string",
			            	CreateTargetCategoryAndFields = false
			            };
		}

		protected override void Because()
		{
			_report = _sut.Validate(_settings);
		}

		[Test]
		public void It_should_show_a_warning()
		{
			Assert.AreEqual(Severity.Warning, _report.First().Severity);
		}

		[Test]
		public void It_should_return_an_error_message()
		{
			Assert.AreEqual("The category 'talk category' does not exist.", _report.First().Message);
		}
	}

	public class When_the_plugin_settings_are_validated_and_the_category_is_checked : With_existing_category
	{
		void Because(string categoryName)
		{
			base.Because();

			Settings.CategoryName = categoryName;

			_report = _sut.Validate(Settings);
		}

		[RowTest]
		[Row(null)]
		[Row("")]
		[Row("    ")]
		public void It_should_return_an_error_message_for_invalid_values(string categoryName)
		{
			Because(categoryName);
			Assert.AreEqual("Please enter a category name.", _report.First().Message);
		}

		[RowTest]
		[Row(null)]
		[Row("")]
		[Row("    ")]
		public void It_should_return_an_error_for_invalid_values(string categoryName)
		{
			Because(categoryName);
			Assert.AreEqual(Severity.Error, _report.First().Severity);
		}

		[RowTest]
		[Row("Talk category")]
		public void It_return_successfully_for_valid_values(string categoryName)
		{
			Because(categoryName);
			Assert.AreEqual(0, _report.Count);
		}
	}

	public class When_the_plugin_settings_are_validated_and_the_year_query_string_is_checked : With_existing_category
	{
		void Because(string yearQueryString)
		{
			base.Because();

			Settings.YearQueryString = yearQueryString;

			_report = _sut.Validate(Settings);
		}

		[RowTest]
		[Row(null)]
		[Row("")]
		[Row("    ")]
		public void It_should_return_an_error_message_for_invalid_values(string yearQueryString)
		{
			Because(yearQueryString);
			Assert.AreEqual("Please enter a year query string parameter.", _report.First().Message);
		}

		[RowTest]
		[Row(null)]
		[Row("")]
		[Row("    ")]
		public void It_should_return_an_error_for_invalid_values(string yearQueryString)
		{
			Because(yearQueryString);
			Assert.AreEqual(Severity.Error, _report.First().Severity);
		}

		[RowTest]
		[Row("year query string")]
		public void It_return_successfully_for_valid_values(string yearQueryString)
		{
			Because(yearQueryString);
			Assert.AreEqual(0, _report.Count);
		}
	}

	public abstract class With_existing_category : Spec
	{
		protected ValidationReport _report;
		protected IValidator<TalkPluginSettings> _sut;

		protected TalkPluginSettings Settings
		{
			get;
			private set;
		}

		protected override void Establish_context()
		{
			var categoryRepository = MockRepository.GenerateMock<ICategoryRepository>();
			_sut = new TalkPluginSettingsValidator(categoryRepository);

			categoryRepository.Stub(x => x.Exists(null))
				.IgnoreArguments()
				.Return(true);
		}

		protected override void Because()
		{
			Settings = new TalkPluginSettings
			       {
			       	CategoryName = "talk category",
			       	YearQueryString = "year query string",
			       	CreateTargetCategoryAndFields = false,
			       	MigrateFieldValues = false,
			       	// TODO
			       	Date = "date",
			       	// TODO
			       	Speaker = "speaker"
			       };
		}
	}
}