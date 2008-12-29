using System;
using System.Diagnostics;

using Castle.MicroKernel;
using Castle.MicroKernel.Handlers;
using Castle.Windsor;

using DnugLeipzig.Definitions;
using DnugLeipzig.ForTesting;

using MbUnit.Framework;

namespace DnugLeipzig.Container.Tests
{
	public class IoCSpec : Spec
	{
		IHandler[] _sut;

		protected override void Before_each_spec()
		{
			IWindsorContainer container = new WindsorContainer();
			foreach (var registration in Registrations.Get())
			{
				container.Register(registration);
			}
			IoC.Initialize(container);

			_sut = IoC.Container.Kernel.GetAssignableHandlers(typeof(object));
		}

		[Test]
		public void It_should_be_able_to_create_instances_of_registered_types()
		{
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
			              			Debug.WriteLine(handler.ComponentModel.Service + " -> " + handler.ComponentModel.Name);
			              			IoC.Resolve(handler.ComponentModel.Service);
			              		}
			              	});
		}
	}
}