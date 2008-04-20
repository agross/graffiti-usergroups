using System;

using DnugLeipzig.Definitions.Repositories;

using Graffiti.Core;

using MbUnit.Framework;

using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace DnugLeipzig.Plugins.Tests
{
	[TestFixture]
	public class EventPluginTests
	{
		const string DefaultLocation = "Default location value";
		const int EventCategoryId = 2;
		const string LocationField = "Location field";
		const string LocationUnknownField = "Location is unknown field";
		readonly MockRepository _mocks = new MockRepository();
		EventPlugin _plugin;
		Post _post;
		IPostRepository _postRepository;

		[SetUp]
		public void SetUp()
		{
			_postRepository = _mocks.CreateMock<IPostRepository>();

			_plugin = new EventPlugin(_postRepository);
			_plugin.CategoryName = "Events";
			_plugin.LocationField = LocationField;
			_plugin.LocationUnknownField = LocationUnknownField;
			_plugin.DefaultLocation = DefaultLocation;

			_post = new Post();
			_post.CategoryId = EventCategoryId;
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
		[RollBack]
		public void SetsDefaultLocation(string location)
		{
			using (_mocks.Record())
			{
				//_postRepository.Save(null);
				//LastCall.Constraints(Is.Equal(_post));
			}

			using (_mocks.Playback())
			{
				_post.CustomFields().Add(LocationField, location);
				_post.CustomFields().Add(LocationUnknownField, "off");

				_plugin.ga_BeforeValidate(_post, EventArgs.Empty);
				_plugin.ga_SetDefaultValues(_post, EventArgs.Empty);

				Assert.IsNotNull(_post.Custom(LocationField));
				Assert.AreEqual(_post.Custom(LocationField), DefaultLocation);
			}
		}

		[RowTest]
		[Row("some location")]
		public void DoesNotSetDefaultLocationIfLocationIsAvailable(string location)
		{
			_post.CustomFields().Add(LocationField, location);
			_post.CustomFields().Add(LocationUnknownField, "off");

			_plugin.ga_BeforeValidate(_post, EventArgs.Empty);
			_plugin.ga_SetDefaultValues(_post, EventArgs.Empty);

			Assert.IsNotNull(_post.Custom(LocationField));
			Assert.AreEqual(_post.Custom(LocationField), location);
		}

		[Test]
		public void DoesNotSetDefaultLocationIfLocationIsUnknown()
		{
			_post.CustomFields().Add(LocationUnknownField, "on");

			_plugin.ga_BeforeValidate(_post, EventArgs.Empty);
			_plugin.ga_SetDefaultValues(_post, EventArgs.Empty);

			Assert.IsNull(_post.Custom(LocationField));
		}
	}
}