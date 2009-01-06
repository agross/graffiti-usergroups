using DnugLeipzig.ForTesting;

using MbUnit.Framework;

namespace DnugLeipzig.Definitions.Tests
{
	public class When_a_log_message_is_created_with_title_and_message : Spec
	{
		string _result;

		protected override void Because()
		{
			_result = Create.New.LogMessage().WithTitle("title").WithMessage("message");
		}

		[Test]
		public void It_should_create_a_separated_message()
		{
			Assert.AreEqual("title|message", _result);
		}
	}

	public class When_a_log_message_is_created_with_title_only : Spec
	{
		string _result;

		protected override void Because()
		{
			_result = Create.New.LogMessage().WithTitle("title");
		}

		[Test]
		public void It_should_create_a_separated_message()
		{
			Assert.AreEqual("title|", _result);
		}
	}

	public class When_a_log_message_is_created_with_message_only : Spec
	{
		string _result;

		protected override void Because()
		{
			_result = Create.New.LogMessage().WithMessage("message");
		}

		[Test]
		public void It_should_create_a_separated_message()
		{
			Assert.AreEqual("|message", _result);
		}
	}
}