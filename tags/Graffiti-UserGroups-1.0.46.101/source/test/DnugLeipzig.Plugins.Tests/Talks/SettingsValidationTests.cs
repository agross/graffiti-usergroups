using System;
using System.Collections.Specialized;
using System.Web;

using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Plugins.Tests.HttpMocks;

using Graffiti.Core;

using MbUnit.Framework;

using Rhino.Mocks;

namespace DnugLeipzig.Plugins.Tests.Talks
{
	[TestFixture]
	public class SettingsValidationTests
	{
		readonly MockRepository _mocks = new MockRepository();
		ICategoryRepository _categoryRepository;
		TalkPlugin _plugin;
		NameValueCollection _values = new NameValueCollection();

		[SetUp]
		public void SetUp()
		{
			IPostRepository postRepository;
			ISettingsRepository settingsRepository;
			_plugin = SetupHelper.SetUpWithMockedDependencies(_mocks,
															  out _categoryRepository,
															  out settingsRepository,
															  out postRepository);

			_values = new NameValueCollection();
			_values.Add(TalkPlugin.Form_CategoryName, _plugin.CategoryName);
			_values.Add(TalkPlugin.Form_YearQueryString, "year query string");
			_values.Add(TalkPlugin.Form_CreateTargetCategoryAndFields, "off");
		}

		[TearDown]
		public void TearDown()
		{
			_mocks.ReplayAll();
			_mocks.VerifyAll();
		}

		[RowTest]
		[Row(null, StatusType.Error)]
		[Row("", StatusType.Error)]
		[Row("    ", StatusType.Error)]
		[Row(SetupHelper.TalkCategoryName, StatusType.Success)]
		public void RequiresCategoryName(string categoryName, StatusType expectedStatus)
		{
			_values[TalkPlugin.Form_CategoryName] = categoryName;

			using (_mocks.Record())
			{
				SetupResult.For(_categoryRepository.IsExistingCategory(_plugin.CategoryName)).Return(true);
			}

			using (_mocks.Playback())
			{
				using (new HttpSimulator().SimulateRequest())
				{
					StatusType status = _plugin.SetValues(HttpContext.Current, _values);

					Assert.AreEqual(expectedStatus, status, "Should have set correct status.");
					if (expectedStatus == StatusType.Error)
					{
						Assert.AreEqual(HttpContext.Current.Items["PostType-Status-Message"], "Please enter a category name.");
					}
				}
			}
		}

		[Test]
		public void ShowsWarningOnNonExistingCategoryWhenCreateTargetCategoryAndFieldsIsNotChecked()
		{
			_values[TalkPlugin.Form_CategoryName] = _plugin.CategoryName;

			using (_mocks.Record())
			{
				SetupResult.For(_categoryRepository.IsExistingCategory(_plugin.CategoryName)).Return(false);
			}

			using (_mocks.Playback())
			{
				using (new HttpSimulator().SimulateRequest())
				{
					StatusType status = _plugin.SetValues(HttpContext.Current, _values);

					Assert.AreEqual(StatusType.Warning, status, "Should have set warning status due to non-existing category.");
					Assert.AreEqual(HttpContext.Current.Items["PostType-Status-Message"],
					                String.Format("The category '{0}' does not exist.", _plugin.CategoryName));
				}
			}
		}

		[RowTest]
		[Row(null, StatusType.Error)]
		[Row("", StatusType.Error)]
		[Row("    ", StatusType.Error)]
		[Row("year query string", StatusType.Success)]
		public void RequiresYearQueryString(string queryString, StatusType expectedStatus)
		{
			_values[TalkPlugin.Form_YearQueryString] = queryString;

			using (_mocks.Record())
			{
				SetupResult.For(_categoryRepository.IsExistingCategory(_plugin.CategoryName)).Return(true);
			}

			using (_mocks.Playback())
			{
				using (new HttpSimulator().SimulateRequest())
				{
					StatusType status = _plugin.SetValues(HttpContext.Current, _values);

					Assert.AreEqual(expectedStatus, status, "Should have set correct status.");
					if (expectedStatus == StatusType.Error)
					{
						Assert.AreEqual(HttpContext.Current.Items["PostType-Status-Message"],
						                "Please enter a year query string parameter.");
					}
				}
			}
		}
	}
}