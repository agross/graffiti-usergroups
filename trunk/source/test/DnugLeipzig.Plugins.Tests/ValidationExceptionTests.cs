using DnugLeipzig.ForTesting;

using MbUnit.Framework;

namespace DnugLeipzig.Plugins.Tests
{
	public class ValidationExceptionTests : Spec
	{
		[Test]
		public void ConstructsCorrectMessageWithNoAffectedFields()
		{
			ValidationException ex = new ValidationException("Some message.");
			Assert.AreEqual("Some message.", ex.Message);
		}

		[Test]
		public void ConstructsCorrectMessageWithOneAffectedField()
		{
			ValidationException ex = new ValidationException("Some message.", "Field 1");
			Assert.AreEqual("Some message. Affected field: Field 1", ex.Message);
		}

		[Test]
		public void ConstructsCorrectMessageWithMultipleAffectedFields()
		{
			ValidationException ex = new ValidationException("Some message.", "Field 1", "Field 2");
			Assert.AreEqual("Some message. Affected fields: Field 1, Field 2", ex.Message);
		}
	}
}