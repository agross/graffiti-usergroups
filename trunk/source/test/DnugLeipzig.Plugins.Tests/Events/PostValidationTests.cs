using System;

using DnugLeipzig.Definitions.Configuration;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.ForTesting;

using Graffiti.Core;

using MbUnit.Framework;

using Rhino.Mocks;

namespace DnugLeipzig.Plugins.Tests.Events
{
	public class PostValidationTests : Spec
	{
		const string EndDateField = "End date field";
		const string LocationField = "Location field";
		const string LocationUnknownField = "Location is unknown field";
		const string MaximumNumberOfRegistrationsField = "Maximum number of registrations field";
		const string RegistrationRecipientField = "Registration recipient field";
		const string StartDateField = "Start date field";
		readonly MockRepository _mocks = new MockRepository();
		EventPlugin _plugin;
		Post _post;
		IPostRepository _postRepository;

		protected override void Before_each_spec()
		{
			ICategoryRepository categoryRepository;
			IGraffitiSettings settings;
			_plugin = SetupHelper.SetUpWithMockedDependencies(_mocks,
			                                                  out categoryRepository,
			                                                  out settings,
			                                                  out _postRepository);

			_plugin.StartDateField = StartDateField;
			_plugin.EndDateField = EndDateField;
			_plugin.LocationField = LocationField;
			_plugin.LocationUnknownField = LocationUnknownField;
			_plugin.MaximumNumberOfRegistrationsField = MaximumNumberOfRegistrationsField;
			_plugin.RegistrationRecipientField = RegistrationRecipientField;

			_post = new Post { CategoryId = SetupHelper.EventCategoryId };
		}

		[Test]
		public void DoesNotValidateIfItemToBeSavedIsNotAPost()
		{
			DataBuddyBase baseObject = _mocks.PartialMock<DataBuddyBase>();

			using (_mocks.Record())
			{
				// No methods should be called on the mocks.
			}

			using (_mocks.Playback())
			{
				_plugin.Post_Validate(baseObject, EventArgs.Empty);
			}
		}

		[RowTest]
		[Row("")]
		[Row("     ")]
		[Row(null)]
		[Row("2008/2/3")]
		[Row("2008/2/3 8:00 AM")]
		[Row("3.2.2008 8:00")]
		[Row("invalid", ExpectedException = typeof(ValidationException))]
		public void ShouldValidateStartDateIfSet(string startDateValue)
		{
			_post[StartDateField] = startDateValue;

			using (_mocks.Record())
			{
				Expect.Call(_postRepository.GetCategoryNameOf(_post)).Return(_plugin.CategoryName);
			}

			using (_mocks.Playback())
			{
				_plugin.Post_Validate(_post, EventArgs.Empty);
			}
		}

		[RowTest]
		[Row("")]
		[Row("     ")]
		[Row(null)]
		[Row("2008/2/3")]
		[Row("2008/2/3 8:00 AM")]
		[Row("3.2.2008 8:00")]
		[Row("invalid", ExpectedException = typeof(ValidationException))]
		public void ShouldValidateEndDateIfSet(string endDateValue)
		{
			_post[StartDateField] = "2008/2/3";
			_post[EndDateField] = endDateValue;

			using (_mocks.Record())
			{
				Expect.Call(_postRepository.GetCategoryNameOf(_post)).Return(_plugin.CategoryName);
			}

			using (_mocks.Playback())
			{
				_plugin.Post_Validate(_post, EventArgs.Empty);
			}
		}

		[Test]
		[ExpectedException(typeof(ValidationException))]
		public void RequiresStartDateIfEndDateIsSet()
		{
			_post[EndDateField] = "2008/2/3";

			using (_mocks.Record())
			{
				Expect.Call(_postRepository.GetCategoryNameOf(_post)).Return(_plugin.CategoryName);
			}

			using (_mocks.Playback())
			{
				_plugin.Post_Validate(_post, EventArgs.Empty);
			}
		}

		[RowTest]
		[Row("", "")]
		[Row("   ", "    ")]
		[Row(null, null)]
		[Row("2008/2/3", "2008/2/3")]
		[Row("2008/2/3", "2008/2/3 8:00 AM")]
		[Row("2008/2/3 7:00 AM", "2008/2/3 8:00 AM")]
		[Row("2008/2/3 7:00 AM", "2008/2/3", ExpectedException = typeof(ValidationException))]
		[Row("2008/2/3 7:00 AM", "2008/2/3 6:00 AM", ExpectedException = typeof(ValidationException))]
		public void EndDateShouldBeGreaterEqualThanStartDate(string startDateValue, string endDateValue)
		{
			_post[StartDateField] = startDateValue;
			_post[EndDateField] = endDateValue;

			using (_mocks.Record())
			{
				Expect.Call(_postRepository.GetCategoryNameOf(_post)).Return(_plugin.CategoryName);
			}

			using (_mocks.Playback())
			{
				_plugin.Post_Validate(_post, EventArgs.Empty);
			}
		}

		[Test]
		[ExpectedException(typeof(ValidationException))]
		public void LocationUnknownMustBeUnCheckedIfLocationFieldIsNotEmpty()
		{
			_post[LocationField] = "some location";
			_post[LocationUnknownField] = "on";

			using (_mocks.Record())
			{
				Expect.Call(_postRepository.GetCategoryNameOf(_post)).Return(_plugin.CategoryName);
			}

			using (_mocks.Playback())
			{
				_plugin.Post_Validate(_post, EventArgs.Empty);
			}
		}

		[RowTest]
		[Row("")]
		[Row(null)]
		[Row("0")]
		[Row("100")]
		[Row("-100", ExpectedException = typeof(ValidationException))]
		[Row("invalid", ExpectedException = typeof(ValidationException))]
		public void MaximumNumberOfRegistrationsMustBePositiveInteger(string maximumNumberOfRegistrationsValue)
		{
			_post[MaximumNumberOfRegistrationsField] = maximumNumberOfRegistrationsValue;

			using (_mocks.Record())
			{
				Expect.Call(_postRepository.GetCategoryNameOf(_post)).Return(_plugin.CategoryName);
			}

			using (_mocks.Playback())
			{
				_plugin.Post_Validate(_post, EventArgs.Empty);
			}
		}

		[RowTest]
		[Row("")]
		[Row(null)]
		[Row("foo@example.com")]
		[Row("invalid", ExpectedException = typeof(ValidationException))]
		public void RegistrationRecipientMustBeValidIfGiven(string email)
		{
			_post[RegistrationRecipientField] = email;

			using (_mocks.Record())
			{
				Expect.Call(_postRepository.GetCategoryNameOf(_post)).Return(_plugin.CategoryName);
			}

			using (_mocks.Playback())
			{
				_plugin.Post_Validate(_post, EventArgs.Empty);
			}
		}
	}
}