using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Castle.Core;
using Castle.MicroKernel.Registration;

using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Definitions.GraffitiIntegration;
using DnugLeipzig.Definitions.Mapping;
using DnugLeipzig.Definitions.Plugins.Events;
using DnugLeipzig.Definitions.Plugins.Talks;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Definitions.Services;
using DnugLeipzig.Definitions.Validation;
using DnugLeipzig.Runtime.Commands;
using DnugLeipzig.Runtime.GraffitiIntegration;
using DnugLeipzig.Runtime.Plugins.Events;
using DnugLeipzig.Runtime.Plugins.Talks;
using DnugLeipzig.Runtime.Repositories;
using DnugLeipzig.Runtime.Services;

namespace DnugLeipzig.Container
{
	public static class ComponentRegistrations
	{
		public static IEnumerable<IRegistration> Get()
		{
			Assembly runtime = typeof(Command).Assembly;

			yield return Component.For<IClock>()
				.ImplementedBy<Clock>();

			// Mapper.
			yield return AllTypes.Of(typeof(IMapper<,>))
				.FromAssembly(runtime)
				.WithService.Select((type, baseType) => DeepestInterfaceImplementation(type))
				.Configure(r => r.LifeStyle.Is(LifestyleType.Transient));

			// Validators.
			yield return AllTypes.Of(typeof(IValidator<>))
				.FromAssembly(runtime)
				.WithService.Select((type, baseType) => DeepestInterfaceImplementation(type))
				.Configure(r => r.LifeStyle.Is(LifestyleType.Transient));

			// Configuration.
			yield return Component.For<PluginConfigurationInterceptor>()
				.ImplementedBy<PluginConfigurationInterceptor>();

			yield return Component.For<ITalkPluginConfigurationProvider>()
				.ImplementedBy<TalkPlugin>()
				.Interceptors(InterceptorReference.ForType<PluginConfigurationInterceptor>())
				.Anywhere
				.LifeStyle.Is(LifestyleType.Transient);

			yield return Component.For<IEventPluginConfigurationProvider>()
				.ImplementedBy<EventPlugin>()
				.Interceptors(InterceptorReference.ForType<PluginConfigurationInterceptor>())
				.Anywhere
				.LifeStyle.Is(LifestyleType.Transient);

			yield return Component.For<IGraffitiSiteSettings>()
				.ImplementedBy<GraffitiSiteSettings>()
				.LifeStyle.Is(LifestyleType.Transient);

			yield return Component.For<IGraffitiEmailContext>()
				.ImplementedBy<GraffitiEmailContext>()
				.LifeStyle.Is(LifestyleType.Transient);

			// Repositories.
			yield return Component.For<ICalendarItemRepository>()
				.ImplementedBy<CalendarItemRepository>()
				.LifeStyle.Is(LifestyleType.Transient);

			yield return Component.For<IPostRepository>()
				.ImplementedBy<PostRepository>()
				.LifeStyle.Is(LifestyleType.Transient);

			yield return Component.For<ICategoryRepository>()
				.ImplementedBy<CategoryRepository>()
				.LifeStyle.Is(LifestyleType.Transient);

			yield return Component.For<ICategorizedPostRepository<ITalkPluginConfigurationProvider>>()
				.ImplementedBy<CategorizedPostRepository<ITalkPluginConfigurationProvider>>()
				.LifeStyle.Is(LifestyleType.Transient);

			yield return Component.For<ICategorizedPostRepository<IEventPluginConfigurationProvider>>()
				.ImplementedBy<CategorizedPostRepository<IEventPluginConfigurationProvider>>()
				.LifeStyle.Is(LifestyleType.Transient);

			// Commands.
			yield return Component.For<ICommandFactory>()
				.ImplementedBy<CommandFactory>()
				.LifeStyle.Is(LifestyleType.Transient);

			yield return AllTypes.Of<ICommand>()
				.FromAssembly(runtime)
				.WithService.Select((type, baseType) => DeepestInterfaceImplementation(type))
				.Configure(r => r.LifeStyle.Is(LifestyleType.Transient));

			// Services.
			yield return Component.For<IEmailSender>()
				.ImplementedBy<GraffitiEmailSender>()
				.LifeStyle.Is(LifestyleType.Transient);

			yield return Component.For<IEventRegistrationService>()
				.ImplementedBy<EventRegistrationService>()
				.Parameters(new[]
				            {
				            	Parameter.ForKey("registrationEmailTemplate")
				            		.Eq("register.view"),
				            })
				.LifeStyle.Is(LifestyleType.Transient);
		}

		static Type[] DeepestInterfaceImplementation(Type type)
		{
			return new[]
			       {
			       	(type.GetInterfaces().Select(i => new
			       	                                  {
			       	                                  	InterfaceType = i,
			       	                                  	ImplementedInterfaces = i.GetInterfaces().Length
			       	                                  }))
			       		.OrderBy(x => x.ImplementedInterfaces)
			       		.Last()
			       		.InterfaceType
			       };
		}
	}
}