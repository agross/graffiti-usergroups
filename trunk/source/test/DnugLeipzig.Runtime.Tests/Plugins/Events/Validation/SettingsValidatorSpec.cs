using System.Linq;

using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Definitions.Validation;
using DnugLeipzig.ForTesting;
using DnugLeipzig.Runtime.Plugins.Events;
using DnugLeipzig.Runtime.Plugins.Events.Validation;

using MbUnit.Framework;

using Rhino.Mocks;

namespace DnugLeipzig.Runtime.Tests.Plugins.Events.Validation
{
	public class
		When_the_plugin_settings_are_validated_and_the_category_does_not_exists_and_it_should_not_create_the_category
		: Spec
	{
		ValidationReport _report;
		Settings _settings;
		IValidator<Settings> _sut;

		protected override void Establish_context()
		{
			var categoryRepository = MockRepository.GenerateMock<ICategoryRepository>();
			_sut = new SettingsValidator(categoryRepository);

			categoryRepository.Stub(x => x.Exists(null))
				.IgnoreArguments()
				.Return(false);

			_settings = new Settings
			            {
			            	CategoryName = "talk category",
			            	YearQueryString = "year query string",
			            	DefaultMaximumNumberOfRegistrations = "100",
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
		[Row("Talk category")]
		public void It_should_accept_valid_values(string categoryName)
		{
			Because(categoryName);
			Assert.AreEqual(0, _report.Count);
		}

		[RowTest]
		[Row(null)]
		[Row("")]
		[Row("    ")]
		public void It_should_reject_invalid_values(string categoryName)
		{
			Because(categoryName);
			Assert.AreEqual(Severity.Error, _report.First().Severity);
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
		[Row("year query string")]
		public void It_should_accept_valid_values(string yearQueryString)
		{
			Because(yearQueryString);
			Assert.AreEqual(0, _report.Count);
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
		public void It_should_reject_invalid_values(string yearQueryString)
		{
			Because(yearQueryString);
			Assert.AreEqual(Severity.Error, _report.First().Severity);
		}
	}

	public class When_the_plugin_settings_are_validated_and_the_default_registration_recipient_address_is_checked
		: With_existing_category
	{
		void Because(string email)
		{
			base.Because();

			Settings.DefaultRegistrationRecipient = email;

			_report = _sut.Validate(Settings);
		}

		[RowTest]
		[Row(null)]
		[Row("")]
		[Row("   ")]
		[Row("foo@example.com")]
		public void It_should_accept_valid_values(string email)
		{
			Because(email);
			Assert.AreEqual(0, _report.Count);
		}

		[RowTest]
		[Row("invalid")]
		public void It_should_reject_invalid_values(string email)
		{
			Because(email);
			Assert.AreEqual(Severity.Error, _report.First().Severity);
		}

		[RowTest]
		[Row("invalid")]
		public void It_should_return_an_error_message_for_invalid_values(string email)
		{
			Because(email);
			Assert.AreEqual("Please enter a valid e-mail address for the default registration recipient.",
			                _report.First().Message);
		}
	}

	public class When_the_plugin_settings_are_validated_and_the_default_maximum_number_of_registrations_is_checked
		: With_existing_category
	{
		void Because(string numberOfRegistrations)
		{
			base.Because();

			Settings.DefaultMaximumNumberOfRegistrations = numberOfRegistrations;

			_report = _sut.Validate(Settings);
		}

		[RowTest]
		[Row("")]
		[Row(null)]
		[Row("0")]
		[Row("100")]
		public void It_should_accept_valid_values(string numberOfRegistrations)
		{
			Because(numberOfRegistrations);
			Assert.AreEqual(0, _report.Count);
		}

		[RowTest]
		[Row("-100")]
		[Row("invalid")]
		public void It_should_reject_invalid_values(string numberOfRegistrations)
		{
			Because(numberOfRegistrations);
			Assert.AreEqual(Severity.Error, _report.First().Severity);
		}

		[RowTest]
		[Row("-100", "Please enter a value greater than or equal to 0 for the default maximum number of registrations.")]
		[Row("invalid", "Please enter a value greater than or equal to 0 for the default maximum number of registrations.")]
		public void It_should_return_an_error_message_for_invalid_values(string numberOfRegistrations, string message)
		{
			Because(numberOfRegistrations);
			Assert.AreEqual(message, _report.First().Message);
		}
	}

	public abstract class With_existing_category : Spec
	{
		protected ValidationReport _report;
		protected IValidator<Settings> _sut;

		protected Settings Settings
		{
			get;
			private set;
		}

		protected override void Establish_context()
		{
			var categoryRepository = MockRepository.GenerateMock<ICategoryRepository>();
			_sut = new SettingsValidator(categoryRepository);

			categoryRepository.Stub(x => x.Exists(null))
				.IgnoreArguments()
				.Return(true);
		}

		protected override void Because()
		{
			Settings = new Settings
			           {
			           	CategoryName = "talk category",
			           	YearQueryString = "year query string",
			           	DefaultMaximumNumberOfRegistrations = "42",
			           	CreateTargetCategoryAndFields = false,
			           	MigrateFieldValues = false
			           };
			// TODO
			//					DateFormat = "dateFormat";
			//					DefaultLocation = "defaultLocation";
			//					DefaultRegistrationRecipient = "defaultRegistrationRecipient";
			//					EndDate = "endDateField";
			//					Location = "locationField";
			//					LocationUnknown = "locationUnknown";
			//					MaximumNumberOfRegistrations = "maximumNumberOfRegistrations";
			//					RegistrationList = "numberOfRegistrations";
			//					RegistrationMailSubject = "registrationMailSubject";
			//					RegistrationNeeded = "registrationNeeded";
			//					RegistrationRecipient = "registrationRecipient";
			//					ShortEndDateFormat = "shortDateFormat";
			//					Speaker = "speakerField";
			//					StartDate = "startDateField";
			//					UnknownText = "unknownText";
		}
	}
}