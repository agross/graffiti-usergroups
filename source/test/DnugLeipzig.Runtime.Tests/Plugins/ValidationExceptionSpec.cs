using DnugLeipzig.ForTesting;
using DnugLeipzig.Runtime.Plugins;

using MbUnit.Framework;

namespace DnugLeipzig.Runtime.Tests.Plugins
{
	public class When_a_validation_exception_is_created_with_a_message : Spec
	{
		ValidationException _sut;

		protected override void Because()
		{
			_sut = new ValidationException("Some message.");
		}

		[Test]
		public void It_should_create_the_message_without_affected_fields()
		{
			Assert.AreEqual("Some message.", _sut.Message);
		}
	}

	public class When_a_validation_exception_is_created_with_a_message_and_one_affected_field : Spec
	{
		ValidationException _sut;

		protected override void Because()
		{
			_sut = new ValidationException("Some message.", "Field 1");
		}

		[Test]
		public void It_should_create_the_message_with_one_affected_field()
		{
			Assert.AreEqual("Some message. Affected field: Field 1", _sut.Message);
		}
	}

	public class When_a_validation_exception_is_created_with_a_message_and_multiple_affected_fields : Spec
	{
		ValidationException _sut;

		protected override void Because()
		{
			_sut = new ValidationException("Some message.", "Field 1", "Field 2");
		}

		[Test]
		public void It_should_create_the_message_with_multiple_affected_fields()
		{
			Assert.AreEqual("Some message. Affected fields: Field 1, Field 2", _sut.Message);
		}
	}
}