using System.Collections.Generic;

using Castle.MicroKernel.Registration;

using DnugLeipzig.Definitions.Configuration;
using DnugLeipzig.Definitions.Configuration.Plugins;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Definitions.Services;
using DnugLeipzig.Extensions.Configuration;
using DnugLeipzig.Runtime.Configuration;
using DnugLeipzig.Runtime.Repositories;
using DnugLeipzig.Runtime.Services;

namespace DnugLeipzig.Container
{
	public static class Registrations
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

			yield return Component.For<IGraffitiSettings>()
				.ImplementedBy<Settings>()
				.LifeStyle.Transient;

			// Repositories.
			yield return Component.For<IPostRepository>()
				.ImplementedBy<PostRepository>();

			yield return Component.For<ICategoryRepository>()
				.ImplementedBy<CategoryRepository>();

			yield return Component.For<ICategorizedPostRepository<ITalkPluginConfiguration>>()
				.ImplementedBy<CategorizedPostRepository<ITalkPluginConfiguration>>();

			yield return Component.For<ICategorizedPostRepository<IEventPluginConfiguration>>()
				.ImplementedBy<CategorizedPostRepository<IEventPluginConfiguration>>();

			// Services.
			yield return Component.For<IEmailSender>()
				.ImplementedBy<GraffitiEmailSender>();

			yield return Component.For<ICalendarItemService>()
				.ImplementedBy<CalendarItemService>();

			yield return Component.For<IEventRegistrationService>()
				.ImplementedBy<EventRegistrationService>()
				.Parameters(new[]
				            {
				            	Parameter.ForKey("registrationEmailTemplate")
				            		.Eq("register.view"),
				            });
		}
	}
}