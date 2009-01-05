using System;

using DnugLeipzig.Definitions.Configuration;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.ForTesting;
using DnugLeipzig.Runtime.Plugins;

using Graffiti.Core;

using MbUnit.Framework;

using Rhino.Mocks;

namespace DnugLeipzig.Runtime.Tests.Plugins.Events
{
	public class When_the_event_plugin_sets_default_values_on_an_object_that_is_not_a_post : Spec
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
		public void It_should_not_alter_the_object()
		{
			Assert.IsTrue(true);
		}
	}

	public class When_the_event_plugin_sets_default_values_on_a_post_that_is_not_in_the_event_category : Spec
	{
		Post _post;
		EventPlugin _sut;

		protected override void Establish_context()
		{
			var postRepository = MockRepository.GenerateMock<IPostRepository>();
			_sut = new EventPlugin(MockRepository.GenerateMock<ICategoryRepository>(),
			                       postRepository,
			                       MockRepository.GenerateMock<IGraffitiCommentSettings>()) { CategoryName = "Event category" };

			_post = new Post();

			postRepository.Stub(x => x.GetCategoryNameOf(_post)).Return("Some other category");
		}

		protected override void Because()
		{
			_sut.Post_Validate(_post, EventArgs.Empty);
		}

		[Test]
		public void It_should_not_alter_the_post()
		{
			Assert.IsTrue(true);
		}
	}

	public class When_the_event_plugin_sets_default_values_and_the_location_is_empty_and_the_location_is_known
		: With_post_in_category
	{
		void Because(string location)
		{
			_post[LocationField] = location;
			_post[LocationUnknownField] = "off";

			_sut.Post_SetDefaultValues(_post, EventArgs.Empty);
		}

		[RowTest]
		[Row("")]
		[Row("    ")]
		[Row(null)]
		public void It_should_set_the_location(string location)
		{
			Because(location);
			Assert.IsNotNull(_post[LocationField]);
		}

		[RowTest]
		[Row("")]
		[Row("    ")]
		[Row(null)]
		public void It_should_initialize_the_location_with_the_default_location(string location)
		{
			Because(location);
			Assert.AreEqual(DefaultLocation, _post[LocationField]);
		}
	}

	public class When_the_event_plugin_sets_default_values_and_the_location_not_empty : With_post_in_category
	{
		const string Location = "some location";

		void Because(string location)
		{
			_post[LocationField] = location;
			_post[LocationUnknownField] = "off";

			_sut.Post_SetDefaultValues(_post, EventArgs.Empty);
		}

		[Test]
		public void It_should_not_change_the_location()
		{
			Because(Location);
			Assert.AreEqual(Location, _post[LocationField]);
		}
	}

	public class When_the_event_plugin_sets_default_values_and_the_location_unknown : With_post_in_category
	{
		const string Location = "some location";

		void Because(string location)
		{
			_post[LocationField] = location;
			_post[LocationUnknownField] = "on";

			_sut.Post_SetDefaultValues(_post, EventArgs.Empty);
		}

		[Test]
		public void It_should_not_change_the_location()
		{
			Because(Location);
			Assert.AreEqual(Location, _post[LocationField]);
		}

		[Test]
		public void It_should_not_change_the_location_unknown_state()
		{
			Because(Location);
			Assert.AreEqual("on", _post[LocationUnknownField]);
		}
	}

	public class When_the_event_plugin_sets_default_values_and_the_registration_recipient_is_empty : With_post_in_category
	{
		void Because(string registrationRecipient)
		{
			_post[RegistrationRecipientField] = registrationRecipient;

			_sut.Post_SetDefaultValues(_post, EventArgs.Empty);
		}

		[RowTest]
		[Row("")]
		[Row("    ")]
		[Row(null)]
		public void It_should_set_the_registration_recipient(string registrationRecipient)
		{
			Because(registrationRecipient);
			Assert.IsNotNull(_post[RegistrationRecipientField]);
		}

		[RowTest]
		[Row("")]
		[Row("    ")]
		[Row(null)]
		public void It_should_initialize_the_registration_recipient_with_the_default_registration_recipient(string location)
		{
			Because(location);
			Assert.AreEqual(DefaultRegistrationRecipient, _post[RegistrationRecipientField]);
		}
	}

	public class When_the_event_plugin_sets_default_values_and_the_registration_recipient_is_not_empty
		: With_post_in_category
	{
		const string RegistrationRecipient = "registration recipient";

		void Because(string registrationRecipient)
		{
			_post[RegistrationRecipientField] = registrationRecipient;

			_sut.Post_SetDefaultValues(_post, EventArgs.Empty);
		}

		[Test]
		public void It_should_not_change_the_registration_recipient()
		{
			Because(RegistrationRecipient);
			Assert.AreEqual(RegistrationRecipient, _post[RegistrationRecipientField]);
		}
	}

	public class When_the_event_plugin_sets_default_values_and_the_maximum_number_of_registrations_is_empty
		: With_post_in_category
	{
		void Because(string maximumNumberOfRegistrations)
		{
			_post[MaximumNumberOfRegistrationsField] = maximumNumberOfRegistrations;

			_sut.Post_SetDefaultValues(_post, EventArgs.Empty);
		}

		[RowTest]
		[Row("")]
		[Row("    ")]
		[Row(null)]
		public void It_should_set_the_registration_recipient(string maximumNumberOfRegistrations)
		{
			Because(maximumNumberOfRegistrations);
			Assert.IsNotNull(_post[MaximumNumberOfRegistrationsField]);
		}

		[RowTest]
		[Row("")]
		[Row("    ")]
		[Row(null)]
		public void It_should_initialize_the_registration_recipient_with_the_default_registration_recipient(
			string maximumNumberOfRegistrations)
		{
			Because(maximumNumberOfRegistrations);
			Assert.AreEqual(DefaultMaximumNumberOfRegistrations, _post[MaximumNumberOfRegistrationsField]);
		}
	}

	public class When_the_event_plugin_sets_default_values_and_the_maximum_number_of_registrations_is_not_empty
		: With_post_in_category
	{
		const string MaximumNumberOfRegistrations = "42";

		void Because(string maximumNumberOfRegistrations)
		{
			_post[MaximumNumberOfRegistrationsField] = maximumNumberOfRegistrations;

			_sut.Post_SetDefaultValues(_post, EventArgs.Empty);
		}

		[Test]
		public void It_should_not_change_the_registration_recipient()
		{
			Because(MaximumNumberOfRegistrations);
			Assert.AreEqual(MaximumNumberOfRegistrations, _post[MaximumNumberOfRegistrationsField]);
		}
	}

	public abstract class With_post_in_category : Spec
	{
		protected const string DefaultLocation = "Default location value";
		protected const string DefaultMaximumNumberOfRegistrations = "100";
		protected const string DefaultRegistrationRecipient = "Default registration recipient";
		protected const string LocationField = "Location field";
		protected const string LocationUnknownField = "Location is unknown field";
		protected const string MaximumNumberOfRegistrationsField = "Maximum number of registrations field";
		protected const string RegistrationRecipientField = "Registration recipient field";

		protected Post _post;
		protected EventPlugin _sut;

		protected override void Establish_context()
		{
			var postRepository = MockRepository.GenerateMock<IPostRepository>();
			_sut = new EventPlugin(MockRepository.GenerateMock<ICategoryRepository>(),
			                       postRepository,
			                       MockRepository.GenerateMock<IGraffitiCommentSettings>())
			       {
			       	CategoryName = "Event category",
			       	LocationField = LocationField,
			       	LocationUnknownField = LocationUnknownField,
			       	RegistrationRecipientField = RegistrationRecipientField,
			       	MaximumNumberOfRegistrationsField = MaximumNumberOfRegistrationsField,
			       	DefaultLocation = DefaultLocation,
			       	DefaultRegistrationRecipient = DefaultRegistrationRecipient,
			       	DefaultMaximumNumberOfRegistrations = DefaultMaximumNumberOfRegistrations
			       };

			_post = new Post();

			postRepository.Stub(x => x.GetCategoryNameOf(_post)).Return(_sut.CategoryName);
		}
	}
}