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
using DnugLeipzig.Extensions.Configuration;
using DnugLeipzig.Runtime.Commands;
using DnugLeipzig.Runtime.Configuration;
using DnugLeipzig.Runtime.Repositories;
using DnugLeipzig.Runtime.Services;

namespace DnugLeipzig.Container
{
	public static class ComponentRegistrations
	{
		public static IEnumerable<IRegistration> Get()
		{
			// Configurations.
			yield return Component.For<ITalkPluginConfiguration>()
				.ImplementedBy<TalkPluginConfiguration>()
				.LifeStyle.Transient;

			yield return Component.For<IEventPluginConfiguration>()
				.ImplementedBy<EventPluginConfiguration>()
				.LifeStyle.Transient;

			yield return Component.For<IGraffitiCommentSettings>()
				.ImplementedBy<GraffitiCommentSettings>()
				.LifeStyle.Transient;

			yield return Component.For<IGraffitiSiteSettings>()
				.ImplementedBy<GraffitiSiteSettings>()
				.LifeStyle.Transient;

			// Repositories.
			yield return Component.For<ICalendarItemRepository>()
				.ImplementedBy<CalendarItemRepository>();
			
			yield return Component.For<IPostRepository>()
				.ImplementedBy<PostRepository>();

			yield return Component.For<ICategoryRepository>()
				.ImplementedBy<CategoryRepository>();

			yield return Component.For<ICategorizedPostRepository<ITalkPluginConfiguration>>()
				.ImplementedBy<CategorizedPostRepository<ITalkPluginConfiguration>>();

			yield return Component.For<ICategorizedPostRepository<IEventPluginConfiguration>>()
				.ImplementedBy<CategorizedPostRepository<IEventPluginConfiguration>>();

			// Commands.
			yield return Component.For<ICommandFactory>()
				.ImplementedBy<CommandFactory>();

			yield return AllTypes.Of<ICommand>()
				.FromAssembly(typeof(Command).Assembly)
				.WithService.Select((type, baseType) => DeepestInterfaceImplementation(type))
				.Configure(r => r.LifeStyle.Is(LifestyleType.Transient));

			// Services.
			yield return Component.For<IEmailSender>()
				.ImplementedBy<GraffitiEmailSender>();

			yield return Component.For<IEventRegistrationService>()
				.ImplementedBy<EventRegistrationService>()
				.Parameters(new[]
				            {
				            	Parameter.ForKey("registrationEmailTemplate")
				            		.Eq("register.view"),
				            });
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