using System;
using System.Collections.Specialized;
using System.Web;

using DnugLeipzig.Definitions.Configuration;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.ForTesting;
using DnugLeipzig.ForTesting.HttpMocks;

using Graffiti.Core;

using MbUnit.Framework;

using Rhino.Mocks;

namespace DnugLeipzig.Plugins.Tests.Events
{
	public class SettingsValidationTests : Spec
	{
		ICategoryRepository _categoryRepository;
		EventPlugin _plugin;
		NameValueCollection _values = new NameValueCollection();

		protected override void Before_each_spec()
		{
			IPostRepository postRepository;
			IGraffitiSettings settings;
			_plugin = SetupHelper.SetUpWithMockedDependencies(Mocks,
			                                                  out _categoryRepository,
			                                                  out settings,
			                                                  out postRepository);

			_values = new NameValueCollection
			          {
			          	{ EventPlugin.Form_CategoryName, _plugin.CategoryName },
			          	{ EventPlugin.Form_YearQueryString, "year query string" },
			          	{ EventPlugin.Form_DefaultMaximumNumberOfRegistrations, "100" },
			          	{ EventPlugin.Form_CreateTargetCategoryAndFields, "off" }
			          };
		}

		[RowTest]
		[Row(null, StatusType.Error)]
		[Row("", StatusType.Error)]
		[Row("    ", StatusType.Error)]
		[Row(SetupHelper.EventCategoryName, StatusType.Success)]
		public void RequiresCategoryName(string categoryName, StatusType expectedStatus)
		{
			_values[EventPlugin.Form_CategoryName] = categoryName;

			using (Mocks.Record())
			{
				SetupResult.For(_categoryRepository.IsExistingCategory(_plugin.CategoryName)).Return(true);
			}

			using (Mocks.Playback())
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
			_values[EventPlugin.Form_CategoryName] = _plugin.CategoryName;

			using (Mocks.Record())
			{
				SetupResult.For(_categoryRepository.IsExistingCategory(_plugin.CategoryName)).Return(false);
			}

			using (Mocks.Playback())
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
			_values[EventPlugin.Form_YearQueryString] = queryString;

			using (Mocks.Record())
			{
				SetupResult.For(_categoryRepository.IsExistingCategory(_plugin.CategoryName)).Return(true);
			}

			using (Mocks.Playback())
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

		[RowTest]
		[Row(null, StatusType.Success)]
		[Row("", StatusType.Success)]
		[Row("   ", StatusType.Success)]
		[Row("foo@example.com", StatusType.Success)]
		[Row("invalid", StatusType.Error)]
		public void RequiresEmptyOrValidDefaultRegistrationRecipientAddress(string email, StatusType expectedStatus)
		{
			_values[EventPlugin.Form_DefaultRegistrationRecipient] = email;

			using (Mocks.Record())
			{
				SetupResult.For(_categoryRepository.IsExistingCategory(_plugin.CategoryName)).Return(true);
			}

			using (Mocks.Playback())
			{
				using (new HttpSimulator().SimulateRequest())
				{
					StatusType status = _plugin.SetValues(HttpContext.Current, _values);

					Assert.AreEqual(expectedStatus, status, "Should have set correct status.");
					if (expectedStatus == StatusType.Error)
					{
						Assert.AreEqual(HttpContext.Current.Items["PostType-Status-Message"],
						                "Please enter a valid e-mail address for the default registration recipient.");
					}
				}
			}
		}

		[RowTest]
		[Row("", StatusType.Success)]
		[Row(null, StatusType.Success)]
		[Row("0", StatusType.Success)]
		[Row("100", StatusType.Success)]
		[Row("-100", StatusType.Error)]
		[Row("invalid", StatusType.Error)]
		public void DefaultMaximumNumberOfRegistrationsMustEmptyOrBePositiveInteger(string maximumNumberOfRegistrations,
		                                                                            StatusType expectedStatus)
		{
			_values[EventPlugin.Form_DefaultMaximumNumberOfRegistrations] = maximumNumberOfRegistrations;

			using (Mocks.Record())
			{
				SetupResult.For(_categoryRepository.IsExistingCategory(_plugin.CategoryName)).Return(true);
			}

			using (Mocks.Playback())
			{
				using (new HttpSimulator().SimulateRequest())
				{
					StatusType status = _plugin.SetValues(HttpContext.Current, _values);

					Assert.AreEqual(expectedStatus, status, "Should have set correct status.");
					if (expectedStatus == StatusType.Error)
					{
						StringAssert.IsNonEmpty(HttpContext.Current.Items["PostType-Status-Message"] as string);
					}
				}
			}
		}
	}
}