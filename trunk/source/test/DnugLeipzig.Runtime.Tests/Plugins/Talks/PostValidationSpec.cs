using System;

using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.ForTesting;
using DnugLeipzig.Runtime.Plugins;

using Graffiti.Core;

using MbUnit.Framework;

using Rhino.Mocks;

namespace DnugLeipzig.Runtime.Tests.Plugins.Talks
{
	public class When_the_talk_plugin_validates_an_object_that_is_not_a_post : Spec
	{
		DataBuddyBase _post;
		TalkPlugin _sut;

		protected override void Establish_context()
		{
			_sut = new TalkPlugin(MockRepository.GenerateMock<ICategoryRepository>(),
			                      MockRepository.GenerateMock<IPostRepository>());

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

	public class When_the_talk_plugin_validates_a_post_that_is_not_in_the_talk_category : Spec
	{
		Post _post;
		TalkPlugin _sut;

		protected override void Establish_context()
		{
			var postRepository = MockRepository.GenerateMock<IPostRepository>();
			_sut = new TalkPlugin(MockRepository.GenerateMock<ICategoryRepository>(),
			                      postRepository)
			       { CategoryName = "Talk category" };

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

	public class When_the_talk_plugin_validates_a_talk : Spec
	{
		const string DateField = "Date field";
		const string TalkCategory = "Talk category";
		Post _post;

		TalkPlugin _sut;

		protected override void Establish_context()
		{
			var postRepository = MockRepository.GenerateMock<IPostRepository>();
			_sut = new TalkPlugin(MockRepository.GenerateMock<ICategoryRepository>(),
			                      postRepository)
			       {
			       	CategoryName = TalkCategory,
			       	DateField = DateField
			       };

			_post = new Post();

			postRepository.Stub(x => x.GetCategoryNameOf(_post)).Return(TalkCategory);
		}

		void Because(string dateValue)
		{
			_post[DateField] = dateValue;
			_sut.Post_Validate(_post, EventArgs.Empty);
		}

		[RowTest]
		[Row("")]
		[Row("     ")]
		[Row(null)]
		[Row("2008/2/3")]
		[Row("2008/2/3 8:00 AM")]
		[Row("3.2.2008 8:00")]
		public void It_should_validate_correct_talk_dates(string dateValue)
		{
			Because(dateValue);
			Assert.IsTrue(true);
		}

		[RowTest]
		[Row("invalid value", ExpectedException = typeof(ValidationException))]
		public void It_should_not_validate_incorrect_talk_dates(string dateValue)
		{
			Because(dateValue);
			Assert.IsTrue(true);
		}
	}
}