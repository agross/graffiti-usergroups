using System;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Plugins.Events;
using DnugLeipzig.Definitions.Validation;
using DnugLeipzig.ForTesting;
using DnugLeipzig.Runtime.Plugins.Events.Validation;

using Graffiti.Core;

using MbUnit.Framework;

namespace DnugLeipzig.Runtime.Tests.Plugins.Events.Validation
{
	public class When_an_event_is_validated : Spec
	{
		ValidationReport _report;
		IEventValidator _sut;

		protected override void Establish_context()
		{
			_sut = new EventValidator(Create.New.StubbedEventPluginConfiguration().Build());
		}

		void Because(Post post)
		{
			_report = _sut.Validate(post);
		}

		[RowTest]
		[Row("")]
		[Row("     ")]
		[Row(null)]
		public void It_should_accept_empty_start_dates(string date)
		{
			Because(Create.New.Event()
			        	.StartingAt(date));

			Assert.AreEqual(0, _report.Count);
		}

		[RowTest]
		[Row("2008/2/3")]
		[Row("2008/2/3 8:00 AM")]
		[Row("3.2.2008 8:00")]
		public void It_should_accept_valid_start_dates(string date)
		{
			Because(Create.New.Event()
			        	.StartingAt(date)
				);

			Assert.AreEqual(0, _report.Count);
		}

		[RowTest]
		[Row("invalid value")]
		public void It_should_reject_invalid_start_dates(string date)
		{
			Because(Create.New.Event()
			        	.StartingAt(date));

			Assert.IsTrue(_report.HasErrors);
		}

		[RowTest]
		[Row("")]
		[Row("     ")]
		[Row(null)]
		public void It_should_allow_empty_end_dates(string date)
		{
			Because(Create.New.Event()
			        	.StartingAt(DateTime.MinValue)
			        	.To(date));

			Assert.AreEqual(0, _report.Count);
		}

		[RowTest]
		[Row("2008/2/3")]
		[Row("2008/2/3 8:00 AM")]
		[Row("3.2.2008 8:00")]
		public void It_should_accept_valid_end_dates(string date)
		{
			Because(Create.New.Event()
			        	.StartingAt(DateTime.MinValue)
			        	.To(date));

			Assert.AreEqual(0, _report.Count);
		}

		[RowTest]
		[Row("invalid value")]
		public void It_should_reject_invalid_end_dates(string date)
		{
			Because(Create.New.Event()
			        	.StartingAt(DateTime.MinValue)
			        	.To(date)
				);

			Assert.IsTrue(_report.HasErrors);
		}

		[Test]
		public void It_should_reject_an_empty_start_date_if_the_end_date_is_given()
		{
			Because(Create.New.Event()
			        	.StartingAt(null)
			        	.To(new DateTime(2008, 12, 11).ToString())
				);

			Assert.IsTrue(_report.HasErrors);
		}

		[RowTest]
		[Row("", "")]
		[Row("   ", "    ")]
		[Row(null, null)]
		public void It_should_allow_empty_start_and_end_dates_at_the_same_time(string startDate, string endDate)
		{
			Because(Create.New.Event()
			        	.StartingAt(startDate)
			        	.To(endDate)
				);

			Assert.AreEqual(0, _report.Count);
		}

		[RowTest]
		[Row("2008/2/3", "2008/2/3")]
		[Row("2008/2/3", "2008/2/3 8:00 AM")]
		[Row("2008/2/3 7:00 AM", "2008/2/3 8:00 AM")]
		public void It_should_allow_end_dates_after_the_start_date(string startDate, string endDate)
		{
			Because(Create.New.Event()
			        	.StartingAt(startDate)
			        	.To(endDate)
				);

			Assert.AreEqual(0, _report.Count);
		}

		[RowTest]
		[Row("2008/2/3 7:00 AM", "2008/2/3")]
		[Row("2008/2/3 7:00 AM", "2008/2/3 6:00 AM")]
		public void It_should_reject_if_the_start_date_is_greater_than_the_end_date(string startDate, string endDate)
		{
			Because(Create.New.Event()
			        	.StartingAt(startDate)
			        	.To(endDate)
				);

			Assert.IsTrue(_report.HasErrors);
		}

		[RowTest]
		[Row("0")]
		[Row("100")]
		public void It_should_allow_a_positive_integer_for_the_maximum_number_of_registrations(
			string maximumNumberOfRegistrations)
		{
			Because(Create.New.Event()
			        	.WillBeBookedUpAfterRegistrationNumber(maximumNumberOfRegistrations));

			Assert.AreEqual(0, _report.Count);
		}

		[RowTest]
		[Row("-100")]
		[Row("2.5")]
		[Row("invalid")]
		public void It_should_reject_invalid_values_for_the_maximum_number_of_registrations(
			string maximumNumberOfRegistrationsValue)
		{
			Because(Create.New.Event()
			        	.WillBeBookedUpAfterRegistrationNumber(maximumNumberOfRegistrationsValue));

			Assert.IsTrue(_report.HasErrors);
		}

		[RowTest]
		[Row("")]
		[Row(null)]
		public void It_should_allow_an_empty_value_for_the_maximum_number_of_registrations(
			string maximumNumberOfRegistrationsValue)
		{
			Because(Create.New.Event()
			        	.WillBeBookedUpAfterRegistrationNumber(maximumNumberOfRegistrationsValue));

			Assert.AreEqual(0, _report.Count);
		}

		[RowTest]
		[Row("")]
		[Row("   ")]
		[Row(null)]
		public void It_should_allow_an_empty_value_for_the_registration_recipient(string email)
		{
			Because(Create.New.Event()
			        	.OrganizedBy(email));

			Assert.AreEqual(0, _report.Count);
		}

		[RowTest]
		[Row("foo@example.com")]
		public void It_should_allow_valid_email_addresses_for_the_registration_recipient(string email)
		{
			Because(Create.New.Event()
			        	.OrganizedBy(email));

			Assert.AreEqual(0, _report.Count);
		}

		[RowTest]
		[Row("invalid")]
		public void It_should_reject_invalid_email_addresses_for_the_registration_recipient(string email)
		{
			Because(Create.New.Event()
			        	.OrganizedBy(email));

			Assert.IsTrue(_report.HasErrors);
		}
	}
}