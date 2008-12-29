using MbUnit.Framework;

using Rhino.Mocks;

namespace DnugLeipzig.ForTesting
{
	[TestFixture]
	public abstract class Spec
	{
		protected MockRepository Mocks
		{
			get;
			private set;
		}

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			Mocks = new MockRepository();

			Establish_context();
			Because();
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			Mocks.ReplayAll();
			Mocks.VerifyAll();

			Cleanup_after();
		}

		protected virtual void Establish_context()
		{
		}

		protected virtual void Because()
		{
		}

		protected virtual void Cleanup_after()
		{
		}
	}
}