using DnugLeipzig.Definitions;

using Rhino.Testing.AutoMocking;

namespace DnugLeipzig.ForTesting
{
	public class With_IoC_container : Spec
	{
		protected AutoMockingContainer Container
		{
			get;
			set;
		}

		protected override void Establish_context()
		{
			base.Establish_context();
			Mocks.BackToRecordAll();
			Container = new AutoMockingContainer(Mocks);
			Container.Initialize();
			IoC.Initialize(Container);
		}
		
		protected override void Cleanup_after()
		{
			base.Cleanup_after();

			IoC.Reset();
			Container.Dispose();
		}
	}
}