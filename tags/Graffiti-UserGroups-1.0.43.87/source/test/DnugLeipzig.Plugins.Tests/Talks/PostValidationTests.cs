using System;

using DnugLeipzig.Definitions.Repositories;

using Graffiti.Core;

using MbUnit.Framework;

using Rhino.Mocks;

namespace DnugLeipzig.Plugins.Tests.Talks
{
	[TestFixture]
	public class PostValidationTests
	{
		const string DateField = "Date field";
		readonly MockRepository _mocks = new MockRepository();
		TalkPlugin _plugin;
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
			_plugin.DateField = DateField;

			_post = new Post();
			_post.CategoryId = SetupHelper.TalkCategoryId;
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
			_post[DateField] = dateValue;

			using (_mocks.Record())
			{
				Expect.Call(_postRepository.GetCategoryName(_post)).Return(_plugin.CategoryName);
			}

			using (_mocks.Playback())
			{
				_plugin.Post_Validate(_post, EventArgs.Empty);
			}
		}
	}
}