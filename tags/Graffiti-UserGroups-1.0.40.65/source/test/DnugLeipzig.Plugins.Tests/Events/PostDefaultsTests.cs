using System;

using Graffiti.Core;

using MbUnit.Framework;

namespace DnugLeipzig.Plugins.Tests.Events
{
	[TestFixture]
	public class PostDefaultsTests
	{
		const string DefaultLocation = "Default location value";
		const string DefaultMaximumNumberOfRegistrations = "100";
		const string DefaultRegistrationRecipient = "Default registration recipient";
		const int EventCategoryId = 2;
		const string LocationField = "Location field";
		const string LocationUnknownField = "Location is unknown field";
		const string MaximumNumberOfRegistrationsField = "Maximum number of registrations field";
		const string RegistrationRecipientField = "Registration recipient field";
		EventPlugin _plugin;
		Post _post;

		[SetUp]
		public void SetUp()
		{
			_plugin = new EventPlugin();
			_plugin.CategoryName = "Events";
			_plugin.LocationField = LocationField;
			_plugin.LocationUnknownField = LocationUnknownField;
			_plugin.RegistrationRecipientField = RegistrationRecipientField;
			_plugin.MaximumNumberOfRegistrationsField = MaximumNumberOfRegistrationsField;
			_plugin.DefaultLocation = DefaultLocation;
			_plugin.DefaultRegistrationRecipient = DefaultRegistrationRecipient;
			_plugin.DefaultMaximumNumberOfRegistrations = DefaultMaximumNumberOfRegistrations;

			_post = new Post();
			_post.CategoryId = EventCategoryId;
		}

		[RowTest]
		[Row("")]
		[Row("    ")]
		[Row(null)]
		public void SetsDefaultLocation(string location)
		{
			_post.CustomFields().Add(LocationField, location);
			_post.CustomFields().Add(LocationUnknownField, "off");

			_plugin.Post_SetDefaultValues(_post, EventArgs.Empty);

			Assert.IsNotNull(_post[LocationField], "Location should be set to default value.");
			Assert.AreEqual(_post[LocationField], DefaultLocation, "Location should be set to default value.");
		}

		[RowTest]
		[Row("some location")]
		public void DoesNotSetDefaultLocationIfGivenByUser(string location)
		{
			_post.CustomFields().Add(LocationField, location);
			_post.CustomFields().Add(LocationUnknownField, "off");

			_plugin.Post_SetDefaultValues(_post, EventArgs.Empty);

			Assert.IsNotNull(_post[LocationField], "Location should be set to user-supplied value.");
			Assert.AreEqual(_post[LocationField], location, "Location should be set to user-supplied value.");
		}

		[Test]
		public void DoesNotSetDefaultLocationIfLocationIsUnknown()
		{
			_post.CustomFields().Add(LocationUnknownField, "on");

			_plugin.Post_SetDefaultValues(_post, EventArgs.Empty);

			Assert.IsNull(_post[LocationField], "Location should be empty because it's unknown.");
		}

		[RowTest]
		[Row("")]
		[Row("    ")]
		[Row(null)]
		public void SetsDefaultRegistrationRecipient(string registrationRecipient)
		{
			_post.CustomFields().Add(RegistrationRecipientField, registrationRecipient);

			_plugin.Post_SetDefaultValues(_post, EventArgs.Empty);

			Assert.IsNotNull(_post[RegistrationRecipientField], "Registration recipient should be set to default value.");
			Assert.AreEqual(_post[RegistrationRecipientField],
			                DefaultRegistrationRecipient,
			                "Registration recipient should be set to default value.");
		}

		[RowTest]
		[Row("some recipient")]
		public void DoesNotSetDefaultRegistrationRecipientIfGivenByUser(string registrationRecipient)
		{
			_post.CustomFields().Add(RegistrationRecipientField, registrationRecipient);

			_plugin.Post_SetDefaultValues(_post, EventArgs.Empty);

			Assert.IsNotNull(_post[RegistrationRecipientField], "Registration recipient should be set to user-supplied value.");
			Assert.AreEqual(_post[RegistrationRecipientField],
			                registrationRecipient,
			                "Registration recipient should be set to user-supplied value.");
		}

		[RowTest]
		[Row("")]
		[Row("    ")]
		[Row(null)]
		public void SetsDefaultMaximumNumberOfRegistrations(string maximumNumberOfRegistrations)
		{
			_post.CustomFields().Add(MaximumNumberOfRegistrationsField, maximumNumberOfRegistrations);

			_plugin.Post_SetDefaultValues(_post, EventArgs.Empty);

			Assert.IsNotNull(_post[MaximumNumberOfRegistrationsField],
			                 "Maximum number of registrations should be set to default value.");
			Assert.AreEqual(_post[MaximumNumberOfRegistrationsField],
			                DefaultMaximumNumberOfRegistrations,
			                "Maximum number of registrations should be set to default value.");
		}

		[RowTest]
		[Row("42")]
		public void DoesNotSetDefaultMaximumNumberOfRegistrationsIfGivenByUser(string maximumNumberOfRegistrations)
		{
			_post.CustomFields().Add(MaximumNumberOfRegistrationsField, maximumNumberOfRegistrations);

			_plugin.Post_SetDefaultValues(_post, EventArgs.Empty);

			Assert.IsNotNull(_post[MaximumNumberOfRegistrationsField],
			                 "Maximum number of registrations should be set to user-supplied value.");
			Assert.AreEqual(_post[MaximumNumberOfRegistrationsField],
			                maximumNumberOfRegistrations,
			                "Maximum number of registrations should be set to user-supplied value.");
		}
	}
}