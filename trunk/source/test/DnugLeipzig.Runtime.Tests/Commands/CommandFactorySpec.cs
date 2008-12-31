using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.ForTesting;
using DnugLeipzig.Runtime.Commands;

using MbUnit.Framework;

using Rhino.Testing.AutoMocking;

namespace DnugLeipzig.Runtime.Tests.Commands
{
	public class When_the_command_factory_builds_a_registration_request : With_command_factory<IEventRegistrationCommand>
	{
		protected override void Because()
		{
			_command = _factory.EventRegistration(new[] { 10, 42 },
			                                      "firstname lastname",
			                                      "form of address",
			                                      "occupation",
			                                      "attendee@email.com",
			                                      true);
		}
	}

	public abstract class With_command_factory<TCommand> : Spec where TCommand : class, ICommand
	{
		protected TCommand _command;
		protected CommandFactory _factory;

		protected override void Establish_context()
		{
			var container = new AutoMockingContainer(Mocks);
			container.Initialize();
			container.Mark<TCommand>().Stubbed();
			IoC.Initialize(container);

			_factory = new CommandFactory();
		}

		protected override void Cleanup_after()
		{
			IoC.Container.Dispose();
			IoC.Reset();
		}

		[Test]
		public void It_should_create_a_command()
		{
			Assert.IsNotNull(_command);
		}
	}
}