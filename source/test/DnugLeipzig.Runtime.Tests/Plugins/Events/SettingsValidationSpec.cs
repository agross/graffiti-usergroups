using System;
using System.Collections.Specialized;
using System.Web;

using DnugLeipzig.Definitions.Configuration;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.ForTesting;
using DnugLeipzig.ForTesting.HttpMocks;
using DnugLeipzig.Runtime.Plugins;

using Graffiti.Core;

using MbUnit.Framework;

using Rhino.Mocks;

namespace DnugLeipzig.Runtime.Tests.Plugins.Events
{
	public class
		When_the_plugin_settings_are_validated_and_the_selected_category_does_not_exists_and_it_should_not_create_the_category
		: Spec
	{
		HttpSimulator _request;
		StatusType _status;
		EventPlugin _sut;

		protected override void Establish_context()
		{
			var categoryRepository = MockRepository.GenerateMock<ICategoryRepository>();
			_sut = new EventPlugin(categoryRepository,
			                       MockRepository.GenerateMock<IPostRepository>(),
			                       MockRepository.GenerateMock<IGraffitiCommentSettings>()) { CategoryName = "Talk category" };

			var form = new NameValueCollection
			           {
			           	{ EventPlugin.Form_CategoryName, _sut.CategoryName },
			           	{ EventPlugin.Form_YearQueryString, "year query string" },
			           	{ EventPlugin.Form_DefaultMaximumNumberOfRegistrations, "100" },
			           	{ EventPlugin.Form_CreateTargetCategoryAndFields, "off" }
			           };

			_request = new HttpSimulator().SimulateRequest(new Uri("http://foo"), form);

			categoryRepository.Stub(x => x.IsExistingCategory(_sut.CategoryName)).Return(false);
		}

		protected override void Because()
		{
			_status = _sut.SetValues(HttpContext.Current, HttpContext.Current.Request.Form);
		}

		protected override void Cleanup_after()
		{
			_request.Dispose();
		}

		[Test]
		public void It_should_show_a_warning()
		{
			Assert.AreEqual(StatusType.Warning, _status);
		}

		[Test]
		public void It_should_show_a_warning_message()
		{
			Assert.AreEqual(HttpContext.Current.Items["PostType-Status-Message"],
			                String.Format("The category '{0}' does not exist.", _sut.CategoryName));
		}
	}

	public class When_the_plugin_settings_are_validated_and_the_category_is_checked : With_existing_category
	{
		void Because(string categoryName)
		{
			var form = new NameValueCollection
			           {
			           	{ EventPlugin.Form_CategoryName, categoryName },
			           	{ EventPlugin.Form_YearQueryString, "year query string" },
			           	{ EventPlugin.Form_DefaultMaximumNumberOfRegistrations, "100" },
			           	{ EventPlugin.Form_CreateTargetCategoryAndFields, "off" }
			           };

			using (new HttpSimulator().SimulateRequest(new Uri("http://foo"), form))
			{
				_status = _sut.SetValues(HttpContext.Current, HttpContext.Current.Request.Form);
				_statusMessage = HttpContext.Current.Items["PostType-Status-Message"];
			}
		}

		[RowTest]
		[Row(null)]
		[Row("")]
		[Row("    ")]
		public void It_should_show_an_error_for_invalid_values(string categoryName)
		{
			Because(categoryName);
			Assert.AreEqual(StatusType.Error, _status);
		}

		[RowTest]
		[Row(null)]
		[Row("")]
		[Row("    ")]
		public void It_should_show_an_error_message_for_invalid_values(string categoryName)
		{
			Because(categoryName);
			Assert.AreEqual("Please enter a category name.", _statusMessage);
		}

		[RowTest]
		[Row("Talk category")]
		public void It_should_show_a_success_message_for_valid_values(string categoryName)
		{
			Because(categoryName);
			Assert.AreEqual(StatusType.Success, _status);
		}
	}

	public class When_the_plugin_settings_are_validated_and_the_year_query_string_is_checked : With_existing_category
	{
		void Because(string yearQueryString)
		{
			var form = new NameValueCollection
			           {
			           	{ EventPlugin.Form_CategoryName, _sut.CategoryName },
			           	{ EventPlugin.Form_YearQueryString, yearQueryString },
			           	{ EventPlugin.Form_DefaultMaximumNumberOfRegistrations, "100" },
			           	{ EventPlugin.Form_CreateTargetCategoryAndFields, "off" }
			           };

			using (new HttpSimulator().SimulateRequest(new Uri("http://foo"), form))
			{
				_status = _sut.SetValues(HttpContext.Current, HttpContext.Current.Request.Form);
				_statusMessage = HttpContext.Current.Items["PostType-Status-Message"];
			}
		}

		[RowTest]
		[Row(null)]
		[Row("")]
		[Row("    ")]
		public void It_should_show_an_error_for_invalid_values(string yearQueryString)
		{
			Because(yearQueryString);
			Assert.AreEqual(StatusType.Error, _status);
		}

		[RowTest]
		[Row(null)]
		[Row("")]
		[Row("    ")]
		public void It_should_show_an_error_message_for_invalid_values(string yearQueryString)
		{
			Because(yearQueryString);
			Assert.AreEqual("Please enter a year query string parameter.", _statusMessage);
		}

		[RowTest]
		[Row("year query string")]
		public void It_should_show_a_success_message_for_valid_values(string yearQueryString)
		{
			Because(yearQueryString);
			Assert.AreEqual(StatusType.Success, _status);
		}
	}

	public class When_the_plugin_settings_are_validated_and_the_default_registration_recipient_address_is_checked
		: With_existing_category
	{
		void Because(string email)
		{
			var form = new NameValueCollection
			           {
			           	{ EventPlugin.Form_CategoryName, _sut.CategoryName },
			           	{ EventPlugin.Form_YearQueryString, "year query string" },
			           	{ EventPlugin.Form_DefaultMaximumNumberOfRegistrations, "100" },
			           	{ EventPlugin.Form_DefaultRegistrationRecipient, email },
			           	{ EventPlugin.Form_CreateTargetCategoryAndFields, "off" }
			           };

			using (new HttpSimulator().SimulateRequest(new Uri("http://foo"), form))
			{
				_status = _sut.SetValues(HttpContext.Current, HttpContext.Current.Request.Form);
				_statusMessage = HttpContext.Current.Items["PostType-Status-Message"];
			}
		}

		[RowTest]
		[Row("invalid")]
		public void It_should_show_an_error_for_invalid_values(string email)
		{
			Because(email);
			Assert.AreEqual(StatusType.Error, _status);
		}

		[RowTest]
		[Row("invalid")]
		public void It_should_show_an_error_message_for_invalid_values(string email)
		{
			Because(email);
			Assert.AreEqual("Please enter a valid e-mail address for the default registration recipient.", _statusMessage);
		}

		[RowTest]
		[Row(null)]
		[Row("")]
		[Row("   ")]
		[Row("foo@example.com")]
		public void It_should_show_a_success_message_for_valid_values(string email)
		{
			Because(email);
			Assert.AreEqual(StatusType.Success, _status);
		}
	}

	public class When_the_plugin_settings_are_validated_and_the_default_maximum_number_of_registrations_is_checked
		: With_existing_category
	{
		void Because(string numberOfRegistrations)
		{
			var form = new NameValueCollection
			           {
			           	{ EventPlugin.Form_CategoryName, _sut.CategoryName },
			           	{ EventPlugin.Form_YearQueryString, "year query string" },
			           	{ EventPlugin.Form_DefaultMaximumNumberOfRegistrations, numberOfRegistrations },
			           	{ EventPlugin.Form_CreateTargetCategoryAndFields, "off" }
			           };

			using (new HttpSimulator().SimulateRequest(new Uri("http://foo"), form))
			{
				_status = _sut.SetValues(HttpContext.Current, HttpContext.Current.Request.Form);
				_statusMessage = HttpContext.Current.Items["PostType-Status-Message"];
			}
		}

		[RowTest]
		[Row("-100")]
		[Row("invalid")]
		public void It_should_show_an_error_for_invalid_values(string numberOfRegistrations)
		{
			Because(numberOfRegistrations);
			Assert.AreEqual(StatusType.Error, _status);
		}

		[RowTest]
		[Row("-100")]
		[Row("invalid")]
		public void It_should_show_an_error_message_for_invalid_values(string numberOfRegistrations)
		{
			Because(numberOfRegistrations);
			StringAssert.IsNonEmpty(_statusMessage as string);
		}

		[RowTest]
		[Row("")]
		[Row(null)]
		[Row("0")]
		[Row("100")]
		public void It_should_show_a_success_message_for_valid_values(string numberOfRegistrations)
		{
			Because(numberOfRegistrations);
			Assert.AreEqual(StatusType.Success, _status);
		}
	}

	public abstract class With_existing_category : Spec
	{
		protected StatusType _status;
		protected object _statusMessage;
		protected EventPlugin _sut;

		protected override void Establish_context()
		{
			var categoryRepository = MockRepository.GenerateMock<ICategoryRepository>();
			_sut = new EventPlugin(categoryRepository,
			                       MockRepository.GenerateMock<IPostRepository>(),
			                       MockRepository.GenerateMock<IGraffitiCommentSettings>()) { CategoryName = "Talk category" };

			categoryRepository.Stub(x => x.IsExistingCategory(null))
				.IgnoreArguments()
				.Return(true);
		}
	}
}