using Rhino.Mocks;

namespace DnugLeipzig.ForTesting
{
	public abstract class AbstractSpec
	{
		MockRepository _mocks = new MockRepository();

		protected MockRepository Mocks
		{
			get
			{
				if (StaticMocks != null)
				{
					return StaticMocks;
				}
				return _mocks;
			}
		}

		/// <summary>
		/// Gets the static mocks.
		/// </summary>
		/// <value>The static mocks.</value>
		/// <remarks>Override this member if you want to use CombinatorialTests with factory
		/// methods that created instances of systems under test with mocked dependencies.</remarks>
		protected virtual MockRepository StaticMocks
		{
			get { return null; }
		}

		public virtual void SetUp()
		{
			_mocks = new MockRepository();
		}

		public virtual void TearDown()
		{
			try
			{
				Mocks.ReplayAll();
				Mocks.VerifyAll();
			}
			finally
			{
				if (StaticMocks != null)
				{
					Mocks.BackToRecordAll();
				}
			}
		}
	}
}