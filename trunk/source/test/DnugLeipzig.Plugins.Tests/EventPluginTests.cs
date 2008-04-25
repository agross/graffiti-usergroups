using System;

using Graffiti.Core;

using MbUnit.Framework;

namespace DnugLeipzig.Plugins.Tests
{
	[TestFixture]
	public class EventPluginTests
	{
		const string DefaultLocation = "Default location value";
		const int EventCategoryId = 2;
		const string LocationField = "Location field";
		const string LocationUnknownField = "Location is unknown field";
		EventPlugin _plugin;
		Post _post;

		[SetUp]
		public void SetUp()
		{
			_plugin = new EventPlugin();
			_plugin.CategoryName = "Events";
			_plugin.LocationField = LocationField;
			_plugin.LocationUnknownField = LocationUnknownField;
			_plugin.DefaultLocation = DefaultLocation;

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

			_plugin.Post_Validate(_post, EventArgs.Empty);
			_plugin.Post_SetDefaultValues(_post, EventArgs.Empty);

			Assert.IsNotNull(_post[LocationField]);
			Assert.AreEqual(_post[LocationField], DefaultLocation);
		}

		[RowTest]
		[Row("some location")]
		public void DoesNotSetDefaultLocationIfLocationIsAvailable(string location)
		{
			_post.CustomFields().Add(LocationField, location);
			_post.CustomFields().Add(LocationUnknownField, "off");

			_plugin.Post_Validate(_post, EventArgs.Empty);
			_plugin.Post_SetDefaultValues(_post, EventArgs.Empty);

			Assert.IsNotNull(_post[LocationField]);
			Assert.AreEqual(_post[LocationField], location);
		}

		[Test]
		public void DoesNotSetDefaultLocationIfLocationIsUnknown()
		{
			_post.CustomFields().Add(LocationUnknownField, "on");

			_plugin.Post_Validate(_post, EventArgs.Empty);
			_plugin.Post_SetDefaultValues(_post, EventArgs.Empty);

			Assert.IsNull(_post[LocationField]);
		}
	}
}