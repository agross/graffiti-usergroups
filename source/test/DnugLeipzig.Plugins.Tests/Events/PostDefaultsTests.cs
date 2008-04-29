using System;

using DnugLeipzig.Definitions.Repositories;

using Graffiti.Core;

using MbUnit.Framework;

using Rhino.Mocks;

namespace DnugLeipzig.Plugins.Tests.Events
{
	[TestFixture]
	public class PostDefaultsTests
	{
		const string DefaultLocation = "Default location value";
		const string DefaultMaximumNumberOfRegistrations = "100";
		const string DefaultRegistrationRecipient = "Default registration recipient";
		const string LocationField = "Location field";
		const string LocationUnknownField = "Location is unknown field";
		const string MaximumNumberOfRegistrationsField = "Maximum number of registrations field";
		const string RegistrationRecipientField = "Registration recipient field";
		readonly MockRepository _mocks = new MockRepository();
		EventPlugin _plugin;
		Post _post;
		IPostRepository _postRepository;

		[SetUp]
		public void SetUp()
		{
			ICategoryRepository categoryRepository;
			ISettingsRepository settingsRepository;
			_plugin = SetupHelper.SetUpWithMockedDependencies(_mocks,
			                                                  out categoryRepository,
			                                                  out settingsRepository,
			                                                  out _postRepository);

			_plugin.LocationField = LocationField;
			_plugin.LocationUnknownField = LocationUnknownField;
			_plugin.RegistrationRecipientField = RegistrationRecipientField;
			_plugin.MaximumNumberOfRegistrationsField = MaximumNumberOfRegistrationsField;
			_plugin.DefaultLocation = DefaultLocation;
			_plugin.DefaultRegistrationRecipient = DefaultRegistrationRecipient;
			_plugin.DefaultMaximumNumberOfRegistrations = DefaultMaximumNumberOfRegistrations;

			_post = new Post();
			_post.CategoryId = SetupHelper.EventCategoryId;
		}

		[TearDown]
		public void TearDown()
		{
			_mocks.ReplayAll();
			_mocks.VerifyAll();
		}

		[RowTest]
		[Row("")]
		[Row("    ")]
		[Row(null)]
		public void SetsDefaultLocation(string location)
		{
			_post[LocationField] = location;
			_post[LocationUnknownField] = "off";

			using (_mocks.Record())
			{
				Expect.Call(_postRepository.GetCategoryName(_post)).Return(_plugin.CategoryName);
			}

			using (_mocks.Playback())
			{
				_plugin.Post_SetDefaultValues(_post, EventArgs.Empty);

				Assert.IsNotNull(_post[LocationField], "Location should be set to default value.");
				Assert.AreEqual(_post[LocationField], DefaultLocation, "Location should be set to default value.");
			}
		}

		[RowTest]
		[Row("some location")]
		public void DoesNotSetDefaultLocationIfGivenByUser(string location)
		{
			_post[LocationField] = location;
			_post[LocationUnknownField] = "off";

			using (_mocks.Record())
			{
				Expect.Call(_postRepository.GetCategoryName(_post)).Return(_plugin.CategoryName);
			}

			using (_mocks.Playback())
			{
				_plugin.Post_SetDefaultValues(_post, EventArgs.Empty);

				Assert.IsNotNull(_post[LocationField], "Location should be set to user-supplied value.");
				Assert.AreEqual(_post[LocationField], location, "Location should be set to user-supplied value.");
			}
		}

		[Test]
		public void DoesNotSetDefaultLocationIfLocationIsUnknown()
		{
			_post[LocationUnknownField] = "on";

			using (_mocks.Record())
			{
				Expect.Call(_postRepository.GetCategoryName(_post)).Return(_plugin.CategoryName);
			}

			using (_mocks.Playback())
			{
				_plugin.Post_SetDefaultValues(_post, EventArgs.Empty);

				Assert.IsNull(_post[LocationField], "Location should be empty because it's unknown.");
			}
		}

		[RowTest]
		[Row("")]
		[Row("    ")]
		[Row(null)]
		public void SetsDefaultRegistrationRecipient(string registrationRecipient)
		{
			_post[RegistrationRecipientField] = registrationRecipient;

			using (_mocks.Record())
			{
				Expect.Call(_postRepository.GetCategoryName(_post)).Return(_plugin.CategoryName);
			}

			using (_mocks.Playback())
			{
				_plugin.Post_SetDefaultValues(_post, EventArgs.Empty);

				Assert.IsNotNull(_post[RegistrationRecipientField], "Registration recipient should be set to default value.");
				Assert.AreEqual(_post[RegistrationRecipientField],
				                DefaultRegistrationRecipient,
				                "Registration recipient should be set to default value.");
			}
		}

		[RowTest]
		[Row("some recipient")]
		public void DoesNotSetDefaultRegistrationRecipientIfGivenByUser(string registrationRecipient)
		{
			_post[RegistrationRecipientField] = registrationRecipient;

			using (_mocks.Record())
			{
				Expect.Call(_postRepository.GetCategoryName(_post)).Return(_plugin.CategoryName);
			}

			using (_mocks.Playback())
			{
				_plugin.Post_SetDefaultValues(_post, EventArgs.Empty);

				Assert.IsNotNull(_post[RegistrationRecipientField], "Registration recipient should be set to user-supplied value.");
				Assert.AreEqual(_post[RegistrationRecipientField],
				                registrationRecipient,
				                "Registration recipient should be set to user-supplied value.");
			}
		}

		[RowTest]
		[Row("")]
		[Row("    ")]
		[Row(null)]
		public void SetsDefaultMaximumNumberOfRegistrations(string maximumNumberOfRegistrations)
		{
			_post[MaximumNumberOfRegistrationsField] = maximumNumberOfRegistrations;

			using (_mocks.Record())
			{
				Expect.Call(_postRepository.GetCategoryName(_post)).Return(_plugin.CategoryName);
			}

			using (_mocks.Playback())
			{
				_plugin.Post_SetDefaultValues(_post, EventArgs.Empty);

				Assert.IsNotNull(_post[MaximumNumberOfRegistrationsField],
				                 "Maximum number of registrations should be set to default value.");
				Assert.AreEqual(_post[MaximumNumberOfRegistrationsField],
				                DefaultMaximumNumberOfRegistrations,
				                "Maximum number of registrations should be set to default value.");
			}
		}

		[RowTest]
		[Row("42")]
		public void DoesNotSetDefaultMaximumNumberOfRegistrationsIfGivenByUser(string maximumNumberOfRegistrations)
		{
			_post[MaximumNumberOfRegistrationsField] = maximumNumberOfRegistrations;

			using (_mocks.Record())
			{
				Expect.Call(_postRepository.GetCategoryName(_post)).Return(_plugin.CategoryName);
			}

			using (_mocks.Playback())
			{
				_plugin.Post_SetDefaultValues(_post, EventArgs.Empty);

				Assert.IsNotNull(_post[MaximumNumberOfRegistrationsField],
				                 "Maximum number of registrations should be set to user-supplied value.");
				Assert.AreEqual(_post[MaximumNumberOfRegistrationsField],
				                maximumNumberOfRegistrations,
				                "Maximum number of registrations should be set to user-supplied value.");
			}
		}
	}
}