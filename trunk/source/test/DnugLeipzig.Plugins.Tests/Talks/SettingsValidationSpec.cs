using System;
using System.Collections.Specialized;
using System.Web;

using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.ForTesting;
using DnugLeipzig.ForTesting.HttpMocks;

using Graffiti.Core;

using MbUnit.Framework;

using Rhino.Mocks;

namespace DnugLeipzig.Plugins.Tests.Talks
{
	public class
		When_the_plugin_settings_are_validated_and_the_selected_category_does_not_exists_and_it_should_not_create_the_category
		: Spec
	{
		HttpSimulator _request;
		StatusType _status;
		TalkPlugin _sut;

		protected override void Establish_context()
		{
			var categoryRepository = MockRepository.GenerateMock<ICategoryRepository>();
			_sut = new TalkPlugin(categoryRepository,
			                      MockRepository.GenerateMock<IPostRepository>()) { CategoryName = "Talk category" };

			var form = new NameValueCollection
			           {
			           	{ TalkPlugin.Form_CategoryName, _sut.CategoryName },
			           	{ TalkPlugin.Form_YearQueryString, "year query string" },
			           	{ TalkPlugin.Form_CreateTargetCategoryAndFields, "off" }
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
			           	{ TalkPlugin.Form_CategoryName, categoryName },
			           	{ TalkPlugin.Form_YearQueryString, "year query string" },
			           	{ TalkPlugin.Form_CreateTargetCategoryAndFields, "off" }
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
			           	{ TalkPlugin.Form_CategoryName, "Talk category" },
			           	{ TalkPlugin.Form_YearQueryString, yearQueryString },
			           	{ TalkPlugin.Form_CreateTargetCategoryAndFields, "off" }
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

	public abstract class With_existing_category : Spec
	{
		protected StatusType _status;
		protected object _statusMessage;
		protected TalkPlugin _sut;

		protected override void Establish_context()
		{
			var categoryRepository = MockRepository.GenerateMock<ICategoryRepository>();
			_sut = new TalkPlugin(categoryRepository,
			                      MockRepository.GenerateMock<IPostRepository>()) { CategoryName = "Talk category" };

			categoryRepository.Stub(x => x.IsExistingCategory(null))
				.IgnoreArguments()
				.Return(true);
		}
	}
}