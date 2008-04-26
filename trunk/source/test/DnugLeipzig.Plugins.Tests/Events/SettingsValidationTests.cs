using System;
using System.Collections.Specialized;
using System.Web;

using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Plugins.Tests.HttpMocks;

using Graffiti.Core;

using MbUnit.Framework;

using Rhino.Mocks;

namespace DnugLeipzig.Plugins.Tests.Events
{
	[TestFixture]
	public class SettingsValidationTests
	{
		const string EventsCategoryName = "Events";
		readonly MockRepository _mocks = new MockRepository();
		ICategoryRepository _categoryRepository;
		EventPlugin _plugin;
		NameValueCollection values = new NameValueCollection();

		[SetUp]
		public void SetUp()
		{
			_categoryRepository = _mocks.CreateMock<ICategoryRepository>();
			_plugin = new EventPlugin(_categoryRepository);

			values = new NameValueCollection();
			values.Add(EventPlugin.Form_CategoryName, EventsCategoryName);
			values.Add(EventPlugin.Form_YearQueryString, "year query string");
			values.Add(EventPlugin.Form_DefaultMaximumNumberOfRegistrations, "100");
			values.Add(EventPlugin.Form_CreateTargetCategoryAndFields, "off");
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
		[Row(EventsCategoryName, StatusType.Success)]
		public void RequiresCategoryName(string categoryName, StatusType expectedStatus)
		{
			values[EventPlugin.Form_CategoryName] = categoryName;

			using (_mocks.Record())
			{
				SetupResult.For(_categoryRepository.IsExistingCategory(EventsCategoryName)).Return(true);
			}

			using (_mocks.Playback())
			{
				using (new HttpSimulator().SimulateRequest())
				{
					StatusType status = _plugin.SetValues(HttpContext.Current, values);

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
			values[EventPlugin.Form_CategoryName] = EventsCategoryName;

			using (_mocks.Record())
			{
				SetupResult.For(_categoryRepository.IsExistingCategory(EventsCategoryName)).Return(false);
			}

			using (_mocks.Playback())
			{
				using (new HttpSimulator().SimulateRequest())
				{
					StatusType status = _plugin.SetValues(HttpContext.Current, values);

					Assert.AreEqual(StatusType.Warning, status, "Should have set warning status due to non-existing category.");
					Assert.AreEqual(HttpContext.Current.Items["PostType-Status-Message"],
					                String.Format("The category '{0}' does not exist.", EventsCategoryName));
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
			values[EventPlugin.Form_YearQueryString] = queryString;

			using (_mocks.Record())
			{
				SetupResult.For(_categoryRepository.IsExistingCategory(EventsCategoryName)).Return(true);
			}

			using (_mocks.Playback())
			{
				using (new HttpSimulator().SimulateRequest())
				{
					StatusType status = _plugin.SetValues(HttpContext.Current, values);

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
			values[EventPlugin.Form_DefaultRegistrationRecipient] = email;
			using (_mocks.Record())
			{
				SetupResult.For(_categoryRepository.IsExistingCategory(EventsCategoryName)).Return(true);
			}

			using (_mocks.Playback())
			{
				using (new HttpSimulator().SimulateRequest())
				{
					StatusType status = _plugin.SetValues(HttpContext.Current, values);

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
			values[EventPlugin.Form_DefaultMaximumNumberOfRegistrations] = maximumNumberOfRegistrations;

			using (_mocks.Record())
			{
				SetupResult.For(_categoryRepository.IsExistingCategory(EventsCategoryName)).Return(true);
			}

			using (_mocks.Playback())
			{
				using (new HttpSimulator().SimulateRequest())
				{
					StatusType status = _plugin.SetValues(HttpContext.Current, values);

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