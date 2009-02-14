using System;
using System.Collections.Generic;
using System.Diagnostics;

using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Handlers;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.ForTesting;
using DnugLeipzig.Runtime.Logging;
using DnugLeipzig.Runtime.Plugins.Events.Validation;
using DnugLeipzig.Runtime.Plugins.Talks.Validation;

using MbUnit.Framework;

namespace DnugLeipzig.Container.Tests
{
	public class When_the_IoC_container_has_been_initialized : Spec
	{
		IoCModule _module;
		IHandler[] _sut;

		protected override void Establish_context()
		{
			_module = new IoCModule();
			// ReSharper disable AssignNullToNotNullAttribute
			_module.Init(null);
			// ReSharper restore AssignNullToNotNullAttribute
		}

		protected override void Cleanup_after()
		{
			_module.Dispose();
		}

		protected override void Because()
		{
			_sut = IoC.Container.Kernel.GetAssignableHandlers(typeof(object));
		}

		[Test]
		public void It_should_be_able_to_create_instances_of_registered_types()
		{
			IList<Type> typesWithUnsatisfyableCtors = new List<Type> { typeof(EventValidator), typeof(TalkValidator) };

			Array.ForEach(_sut,
			              handler =>
			              	{
			              		if (handler is DefaultGenericHandler)
			              		{
			              			Array.ForEach(handler.ComponentModel.Service.GetGenericArguments(),
			              			              argument => Array.ForEach(argument.GetGenericParameterConstraints(),
			              			                                        constraint =>
			              			                                        	{
			              			                                        		Type type = handler
			              			                                        			.ComponentModel.Service
			              			                                        			.MakeGenericType(constraint);

			              			                                        		Debug.WriteLine(type + " -> " +
			              			                                        		                handler.ComponentModel.Name);

			              			                                        		IoC.Resolve(type);
			              			                                        	})
			              				);
			              		}
			              		else
			              		{
									if (typesWithUnsatisfyableCtors.Contains(handler.ComponentModel.Implementation))
									{
										return;
									}

									Debug.WriteLine(handler.ComponentModel.Service + " -> " + handler.ComponentModel.Name);
			              			IoC.Resolve(handler.ComponentModel.Service);
			              		}
			              	});
		}

		[Test]
		public void It_should_not_contain_services_for_ICommand()
		{
			Assert.IsNull(IoC.TryResolve<ICommand>(), "The container should not contain services for the ICommand interface.");
		}

		[Test]
		public void It_should_only_contain_services_with_a_transient_lifestyle()
		{
			Array.ForEach(_sut,
			              handler =>
			              	{
			              		if (handler.ComponentModel.Service.Assembly != typeof(GraffitiLogger).Assembly)
			              		{
			              			return;
			              		}

			              		Debug.WriteLine(handler.ComponentModel.Service);
			              		Assert.AreEqual(LifestyleType.Transient, handler.ComponentModel.LifestyleType);
			              	});
		}
	}
}