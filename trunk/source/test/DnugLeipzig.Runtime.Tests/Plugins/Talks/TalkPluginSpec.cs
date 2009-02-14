using System;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Plugins.Talks;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Definitions.Validation;
using DnugLeipzig.ForTesting;
using DnugLeipzig.Runtime.Plugins.Talks;
using DnugLeipzig.Runtime.Validation;

using Graffiti.Core;

using MbUnit.Framework;

using Rhino.Mocks;

namespace DnugLeipzig.Runtime.Tests.Plugins.Talks
{
	public class When_the_talk_plugin_validates_an_object_that_is_not_a_post : With_IoC_container
	{
		DataBuddyBase _post;
		TalkPlugin _sut;

		protected override void Establish_context()
		{
			base.Establish_context();

			_sut = Container.Create<TalkPlugin>();

			_post = MockRepository.GenerateStub<DataBuddyBase>();

			Mocks.ReplayAll();
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

	public class When_the_talk_plugin_validates_a_post_that_is_not_in_the_event_category : With_IoC_container
	{
		Post _post;
		TalkPlugin _sut;

		protected override void Establish_context()
		{
			base.Establish_context();

			_sut = Container.Create<TalkPlugin>();
			_sut.CategoryName = "Talk category";

			_post = new Post();

			IoC.Resolve<IPostRepository>().Stub(x => x.GetCategoryNameOf(_post)).Return("Some other category");
			IoC.Resolve<ITalkValidator>()
				.Expect(x => x.Validate(_post))
				.Return(null)
				.Repeat.Never();

			Mocks.ReplayAll();
		}

		protected override void Because()
		{
			_sut.Post_Validate(_post, EventArgs.Empty);
		}

		[Test]
		public void It_should_not_validate_the_post()
		{
			IoC.Resolve<ITalkValidator>().VerifyAllExpectations();
		}
	}

	public class When_the_talk_plugin_successfully_validates_an_event : With_IoC_container
	{
		Post _post;
		TalkPlugin _sut;

		protected override void Establish_context()
		{
			base.Establish_context();

			_sut = Container.Create<TalkPlugin>();
			_sut.CategoryName = "Talk category";

			_post = new Post();

			IoC.Resolve<IPostRepository>().Stub(x => x.GetCategoryNameOf(_post)).Return(_sut.CategoryName);
			IoC.Resolve<ITalkValidator>().Expect(x => x.Validate(_post)).Return(new ValidationReport());

			Mocks.ReplayAll();
		}

		protected override void Because()
		{
			_sut.Post_Validate(_post, EventArgs.Empty);
		}

		[Test]
		public void It_should_validate_the_post_using_the_validator()
		{
			IoC.Resolve<ITalkValidator>().VerifyAllExpectations();
		}
	}

	public class When_the_talk_plugin_fails_to_validate_an_event : With_IoC_container
	{
		Exception _exception;
		Post _post;
		TalkPlugin _sut;

		protected override void Establish_context()
		{
			base.Establish_context();

			_sut = Container.Create<TalkPlugin>();
			_sut.CategoryName = "Talk category";

			_post = new Post();

			IoC.Resolve<IPostRepository>().Stub(x => x.GetCategoryNameOf(_post)).Return(_sut.CategoryName);
			IoC.Resolve<ITalkValidator>().Stub(x => x.Validate(_post)).Return(new ValidationReport
			                                                                   {
			                                                                   	new ValidationError("something"),
			                                                                   	new ValidationError("some other thing")
			                                                                   });

			Mocks.ReplayAll();
		}

		protected override void Because()
		{
			_exception = Catch.Exception(() => _sut.Post_Validate(_post, EventArgs.Empty));
		}

		[Test]
		public void It_should_throw_an_exception()
		{
			Assert.IsNotNull(_exception);
		}

		[Test]
		public void It_should_return_the_first_validation_error_in_the_exception_message()
		{
			Assert.AreEqual("something", _exception.Message);
		}
	}
}