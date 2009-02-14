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
			Container = new AutoMockingContainer(Mocks);
			Container.Initialize();
			IoC.Initialize(Container);
		}
	}
}