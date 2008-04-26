using System;

using Graffiti.Core;

using MbUnit.Framework;

namespace DnugLeipzig.Plugins.Tests
{
	[TestFixture]
	public class TalkPluginPostValidationTests
	{
		const string DateField = "Date field";
		const int TalkCategoryId = 3;
		const string TalksCategoryName = "Talks";
		TalkPlugin _plugin;
		Post _post;

		[SetUp]
		public void SetUp()
		{
			_plugin = new TalkPlugin();
			_plugin.CategoryName = TalksCategoryName;
			_plugin.DateField = DateField;

			_post = new Post();
			_post.CategoryId = TalkCategoryId;
		}

		[RowTest]
		[Row("")]
		[Row("     ")]
		[Row(null)]
		[Row("2008/2/3")]
		[Row("2008/2/3 8:00 AM")]
		[Row("3.2.2008 8:00")]
		[Row("invalid value", ExpectedException = typeof(ValidationException))]
		public void ShouldValidateDateIfSet(string dateValue)
		{
			_post.CustomFields().Add(DateField, dateValue);

			_plugin.Post_Validate(_post, EventArgs.Empty);
		}
	}
}