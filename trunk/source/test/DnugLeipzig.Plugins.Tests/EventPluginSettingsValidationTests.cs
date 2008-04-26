using System;
using System.Collections.Specialized;
using System.Web;

using DnugLeipzig.Plugins.Tests.HttpMocks;

using Graffiti.Core;

using MbUnit.Framework;

namespace DnugLeipzig.Plugins.Tests
{
	[TestFixture]
	public class EventPluginSettingsValidationTests
	{
		const string NonExistingCategoryName = "non-existing category";
		const string EventsCategoryName = "Events";
		EventPlugin _plugin;
		NameValueCollection values = new NameValueCollection();

		[SetUp]
		public void SetUp()
		{
			_plugin = new EventPlugin();

			values = new NameValueCollection();
			values.Add(EventPlugin.Form_CategoryName, EventsCategoryName);
			values.Add(EventPlugin.Form_YearQueryString, "year query string");
			values.Add(EventPlugin.Form_DefaultMaximumNumberOfRegistrations, "10");
			values.Add(EventPlugin.Form_CreateTargetCategoryAndFields, "off");
		}

		[RowTest]
		[Row(null, StatusType.Error)]
		[Row("", StatusType.Error)]
		[Row("    ", StatusType.Error)]
		[Row(EventsCategoryName, StatusType.Success)]
		public void RequiresCategoryName(string categoryName, StatusType expectedStatus)
		{
			values[EventPlugin.Form_CategoryName] = categoryName;

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
		
		[Test]
		public void ShowsWarningOnNonExistingCategoryWhenCreateTargetCategoryAndFieldsIsNotChecked()
		{
			values[EventPlugin.Form_CategoryName] = NonExistingCategoryName;

			using (new HttpSimulator().SimulateRequest())
			{
				StatusType status = _plugin.SetValues(HttpContext.Current, values);

				Assert.AreEqual(StatusType.Warning, status, "Should have set warning status due to non-existing category.");
				Assert.AreEqual(HttpContext.Current.Items["PostType-Status-Message"], String.Format("The category '{0}' does not exist.", NonExistingCategoryName));
			}
		}
		
		[RowTest]
		[Row(null, StatusType.Error)]
		[Row("", StatusType.Error)]
		[Row("    ", StatusType.Error)]
		[Row("year query string", StatusType.Success)]
		public void RequiresYearQueryString(string queryString, StatusType expectedStatus)
		{
			values[EventPlugin.Form_YearQueryString] =  queryString;

			using (new HttpSimulator().SimulateRequest())
			{
				StatusType status = _plugin.SetValues(HttpContext.Current, values);

				Assert.AreEqual(expectedStatus, status, "Should have set correct status.");
				if (expectedStatus == StatusType.Error)
				{
					Assert.AreEqual(HttpContext.Current.Items["PostType-Status-Message"], "Please enter a year query string parameter.");
				}
			}
		}
		
		[RowTest]
		[Row(null, StatusType.Success)]
		[Row("", StatusType.Success)]
		[Row("   ", StatusType.Success)]
		[Row("foo@example.com", StatusType.Success)]
		[Row("invalid", StatusType.Error)]
		public void RequiresValidDefaultRegistrationRecipientAddress(string email, StatusType expectedStatus)
		{
			values[EventPlugin.Form_DefaultRegistrationRecipient] = email;

			using (new HttpSimulator().SimulateRequest())
			{
				StatusType status = _plugin.SetValues(HttpContext.Current, values);

				Assert.AreEqual(expectedStatus, status, "Should have set correct status.");
				if (expectedStatus == StatusType.Error)
				{
					Assert.AreEqual(HttpContext.Current.Items["PostType-Status-Message"], "Please enter a valid e-mail address for the default registration recipient.");
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
		public void DefaultMaximumNumberOfRegistrationsMustBePositiveInteger(string maximumNumberOfRegistrations, StatusType expectedStatus)
		{
			values[EventPlugin.Form_DefaultMaximumNumberOfRegistrations] = maximumNumberOfRegistrations;

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