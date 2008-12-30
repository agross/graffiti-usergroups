using System;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Configuration;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.ForTesting;

using Graffiti.Core;

using MbUnit.Framework;

using Rhino.Mocks;

namespace DnugLeipzig.Plugins.Tests.Events
{
	namespace DnugLeipzig.Plugins.Tests.Talks
	{
		public class When_the_event_plugin_validates_an_object_that_is_not_a_post : Spec
		{
			DataBuddyBase _post;
			EventPlugin _sut;

			protected override void Establish_context()
			{
				_sut = new EventPlugin(MockRepository.GenerateMock<ICategoryRepository>(),
				                       MockRepository.GenerateMock<IPostRepository>(),
									   MockRepository.GenerateMock<IGraffitiCommentSettings>());

				_post = MockRepository.GenerateStub<DataBuddyBase>();
			}

			protected override void Because()
			{
				_sut.Post_Validate(_post, EventArgs.Empty);
			}

			[Test]
			public void It_should_not_validate_the_object()
			{
				Assert.IsTrue(true);
			}
		}

		public class When_the_event_plugin_validates_a_post_that_is_not_in_the_event_category : Spec
		{
			Post _post;
			EventPlugin _sut;

			protected override void Establish_context()
			{
				var postRepository = MockRepository.GenerateMock<IPostRepository>();
				_sut = new EventPlugin(MockRepository.GenerateMock<ICategoryRepository>(),
				                       postRepository,
				                       MockRepository.GenerateMock<IGraffitiCommentSettings>())
				       { CategoryName = "Event category" };

				_post = new Post();

				postRepository.Stub(x => x.GetCategoryNameOf(_post)).Return("Some other category");
			}

			protected override void Because()
			{
				_sut.Post_Validate(_post, EventArgs.Empty);
			}

			[Test]
			public void It_should_not_validate_the_post()
			{
				Assert.IsTrue(true);
			}
		}

		public class When_the_event_plugin_validates_an_event : Spec
		{
			const string EndDateField = "End date field";
			const string EventCategory = "Event category";
			const string LocationField = "Location field";
			const string LocationUnknownField = "Location is unknown field";
			const string MaximumNumberOfRegistrationsField = "Maximum number of registrations field";
			const string RegistrationRecipientField = "Registration recipient field";
			const string StartDateField = "Start date field";

			EventPlugin _sut;

			protected override void Establish_context()
			{
				var postRepository = MockRepository.GenerateMock<IPostRepository>();
				_sut = new EventPlugin(MockRepository.GenerateMock<ICategoryRepository>(),
				                       postRepository,
									   MockRepository.GenerateMock<IGraffitiCommentSettings>())
				       {
				       	CategoryName = EventCategory,
				       	StartDateField = StartDateField,
				       	EndDateField = EndDateField,
				       	LocationField = LocationField,
				       	LocationUnknownField = LocationUnknownField,
				       	MaximumNumberOfRegistrationsField = MaximumNumberOfRegistrationsField,
				       	RegistrationRecipientField = RegistrationRecipientField
				       };

				postRepository.Stub(x => x.GetCategoryNameOf(null))
					.IgnoreArguments()
					.Return(EventCategory);
			}

			void Because(DataBuddyBase post)
			{
				_sut.Post_Validate(post, EventArgs.Empty);
			}

			[RowTest]
			[Row("")]
			[Row("     ")]
			[Row(null)]
			public void It_should_allow_empty_start_dates(string date)
			{
				Because(Create.New.Event(_sut).From(date));
				Assert.IsTrue(true);
			}

			[RowTest]
			[Row("2008/2/3")]
			[Row("2008/2/3 8:00 AM")]
			[Row("3.2.2008 8:00")]
			public void It_should_validate_correct_start_dates(string date)
			{
				Because(Create.New.Event(_sut).From(date));
				Assert.IsTrue(true);
			}

			[RowTest]
			[Row("invalid value", ExpectedException = typeof(ValidationException))]
			public void It_should_not_validate_incorrect_start_dates(string date)
			{
				Because(Create.New.Event(_sut).From(date));
				Assert.IsTrue(true);
			}

			[RowTest]
			[Row("")]
			[Row("     ")]
			[Row(null)]
			public void It_should_allow_empty_end_dates(string date)
			{
				Because(Create.New.Event(_sut)
				        	.From(DateTime.MinValue)
				        	.To(date));
				Assert.IsTrue(true);
			}

			[RowTest]
			[Row("2008/2/3")]
			[Row("2008/2/3 8:00 AM")]
			[Row("3.2.2008 8:00")]
			public void It_should_validate_correct_end_dates(string date)
			{
				Because(Create.New.Event(_sut)
				        	.From(DateTime.MinValue)
				        	.To(date));
				Assert.IsTrue(true);
			}

			[RowTest]
			[Row("invalid value", ExpectedException = typeof(ValidationException))]
			public void It_should_not_validate_incorrect_end_dates(string date)
			{
				Because(Create.New.Event(_sut)
				        	.From(DateTime.MinValue)
				        	.To(date));
				Assert.IsTrue(true);
			}

			[Test]
			[ExpectedException(typeof(ValidationException))]
			public void It_should_require_the_start_date_if_the_end_date_is_given()
			{
				Because(Create.New.Event(_sut).From(null).To(new DateTime(2008, 12, 11).ToString()));
				Assert.IsTrue(true);
			}

			[RowTest]
			[Row("", "")]
			[Row("   ", "    ")]
			[Row(null, null)]
			public void It_should_allow_empty_start_and_end_dates_at_the_same_time(string startDate, string endDate)
			{
				Because(Create.New.Event(_sut).From(startDate).To(endDate));
				Assert.IsTrue(true);
			}

			[RowTest]
			[Row("2008/2/3", "2008/2/3")]
			[Row("2008/2/3", "2008/2/3 8:00 AM")]
			[Row("2008/2/3 7:00 AM", "2008/2/3 8:00 AM")]
			[Row("2008/2/3 7:00 AM", "2008/2/3", ExpectedException = typeof(ValidationException))]
			[Row("2008/2/3 7:00 AM", "2008/2/3 6:00 AM", ExpectedException = typeof(ValidationException))]
			public void It_should_require_that_the_start_date_less_or_equal_than_the_end_date(string startDate, string endDate)
			{
				Because(Create.New.Event(_sut).From(startDate).To(endDate));
				Assert.IsTrue(true);
			}

			[Test]
			[ExpectedException(typeof(ValidationException))]
			public void It_should_require_that_the_location_is_known_if_the_location_is_given()
			{
				Because(Create.New.Event(_sut).AtLocation("some location").LocationIsUnknown());
				Assert.IsTrue(true);
			}

			[RowTest]
			[Row("0")]
			[Row("100")]
			[Row("-100", ExpectedException = typeof(ValidationException))]
			[Row("invalid", ExpectedException = typeof(ValidationException))]
			public void It_should_require_a_positive_integer_for_the_maximum_number_of_registrations(
				string maximumNumberOfRegistrationsValue)
			{
				Because(Create.New.Event(_sut).WithMaximumNumberOfRegistrations(maximumNumberOfRegistrationsValue));
				Assert.IsTrue(true);
			}

			[RowTest]
			[Row("")]
			[Row(null)]
			public void It_should_allow_an_empty_value_for_the_maximum_number_of_registrations(
				string maximumNumberOfRegistrationsValue)
			{
				Because(Create.New.Event(_sut).WithMaximumNumberOfRegistrations(maximumNumberOfRegistrationsValue));
				Assert.IsTrue(true);
			}

			[RowTest]
			[Row("")]
			[Row("   ")]
			[Row(null)]
			public void It_should_allow_an_empty_value_for_the_registration_recipient(string email)
			{
				Because(Create.New.Event(_sut).WithRegistrationRecipient(email));
				Assert.IsTrue(true);
			}

			[RowTest]
			[Row("foo@example.com")]
			[Row("invalid", ExpectedException = typeof(ValidationException))]
			public void It_should_require_valid_email_addresses_for_the_registration_recipient(string email)
			{
				Because(Create.New.Event(_sut).WithRegistrationRecipient(email));
				Assert.IsTrue(true);
			}
		}
	}
}