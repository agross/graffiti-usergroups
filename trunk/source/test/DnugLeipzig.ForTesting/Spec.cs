using MbUnit.Framework;

namespace DnugLeipzig.ForTesting
{
	[TestFixture]
	public abstract class Spec : AbstractSpec
	{
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			Before_each_spec();
		}

		[TearDown]
		public override void TearDown()
		{
			base.TearDown();

			After_each_spec();
		}

		protected virtual void Before_each_spec()
		{
		}

		protected virtual void After_each_spec()
		{
		}
	}
}