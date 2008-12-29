using System;

using DnugLeipzig.Definitions.Configuration;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.ForTesting;

using Graffiti.Core;

using MbUnit.Framework;

using Rhino.Mocks;

namespace DnugLeipzig.Plugins.Tests.Talks
{
	public class PostValidationTests : Spec
	{
		const string DateField = "Date field";
		TalkPlugin _plugin;
		Post _post;
		IPostRepository _postRepository;

		protected override void Before_each_spec()
		{
			ICategoryRepository categoryRepository;
			IGraffitiSettings settings;
			_plugin = SetupHelper.SetUpWithMockedDependencies(Mocks,
			                                                  out categoryRepository,
			                                                  out settings,
			                                                  out _postRepository);
			_plugin.DateField = DateField;

			_post = new Post { CategoryId = SetupHelper.TalkCategoryId };
		}

		[Test]
		public void DoesNotValidateIfItemToBeSavedIsNotAPost()
		{
			DataBuddyBase baseObject = Mocks.PartialMock<DataBuddyBase>();

			using (Mocks.Record())
			{
				// No methods should be called on the mocks.
			}

			using (Mocks.Playback())
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
		[Row("invalid value", ExpectedException = typeof(ValidationException))]
		public void ShouldValidateDateIfSet(string dateValue)
		{
			_post[DateField] = dateValue;

			using (Mocks.Record())
			{
				Expect.Call(_postRepository.GetCategoryNameOf(_post)).Return(_plugin.CategoryName);
			}

			using (Mocks.Playback())
			{
				_plugin.Post_Validate(_post, EventArgs.Empty);
			}
		}
	}
}