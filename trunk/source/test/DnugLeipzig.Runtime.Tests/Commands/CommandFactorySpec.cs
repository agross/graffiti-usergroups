using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.ForTesting;
using DnugLeipzig.Runtime.Commands;

using MbUnit.Framework;

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

	public abstract class With_command_factory<TCommand> : With_IoC_container where TCommand : class, ICommand
	{
		protected TCommand _command;
		protected CommandFactory _factory;

		protected override void Establish_context()
		{
			base.Establish_context();

			Container.Mark<TCommand>().Stubbed();
			_factory = new CommandFactory();
		}

		[Test]
		public void It_should_create_a_command()
		{
			Assert.IsNotNull(_command);
		}
	}
}