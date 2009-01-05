using System;
using System.Collections.Generic;
using System.Linq;

using Castle.Core;
using Castle.MicroKernel.Registration;

using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Definitions.Configuration;
using DnugLeipzig.Definitions.Configuration.Plugins;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Definitions.Services;
using DnugLeipzig.Definitions.Validation;
using DnugLeipzig.Runtime.Commands;
using DnugLeipzig.Runtime.Configuration;
using DnugLeipzig.Runtime.Plugins;
using DnugLeipzig.Runtime.Repositories;
using DnugLeipzig.Runtime.Services;
using DnugLeipzig.Runtime.Validation;

namespace DnugLeipzig.Container
{
	public static class ComponentRegistrations
	{
		public static IEnumerable<IRegistration> Get()
		{
			// Validators.
			yield return Component.For<IValidator<IEventRegistrationCommand>>()
				.ImplementedBy<EventRegistrationCommandValidator>()
				.LifeStyle.Is(LifestyleType.Transient);
			
			// Configuration.
			yield return Component.For<PluginConfigurationInterceptor>()
				.ImplementedBy<PluginConfigurationInterceptor>();

			yield return Component.For<ITalkPluginConfigurationProvider>()
				.ImplementedBy<TalkPlugin>()
				.Interceptors(InterceptorReference.ForType<PluginConfigurationInterceptor>()).Anywhere
				.LifeStyle.Is(LifestyleType.Transient);

			yield return Component.For<IEventPluginConfigurationProvider>()
				.ImplementedBy<EventPlugin>()
				.Interceptors(InterceptorReference.ForType<PluginConfigurationInterceptor>()).Anywhere
				.LifeStyle.Is(LifestyleType.Transient);

			yield return Component.For<IGraffitiCommentSettings>()
				.ImplementedBy<GraffitiCommentSettings>()
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
				.FromAssembly(typeof(Command).Assembly)
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