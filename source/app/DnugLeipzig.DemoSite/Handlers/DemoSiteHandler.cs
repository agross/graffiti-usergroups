using System;
using System.Security;
using System.Threading;
using System.Web;

using DnugLeipzig.Definitions.Configuration;
using DnugLeipzig.Definitions.Extensions;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Plugins;
using DnugLeipzig.Plugins.Migration;
using DnugLeipzig.Runtime.Repositories;

using Graffiti.Core;

namespace DnugLeipzig.DemoSite.Handlers
{
	public class DemoSiteHandler : IHttpHandler
	{
		readonly ICategoryRepository _categoryRepository;
		readonly IPostRepository _postRepository;

		#region Ctors
		public DemoSiteHandler() : this(null, null)
		{
		}

		public DemoSiteHandler(ICategoryRepository categoryRepository, IPostRepository postRepository)
		{
			if (categoryRepository == null)
			{
				categoryRepository = new CategoryRepository();
			}

			if (postRepository == null)
			{
				postRepository = new PostRepository();
			}

			_categoryRepository = categoryRepository;
			_postRepository = postRepository;
		}
		#endregion

		#region IHttpHandler Members
		public void ProcessRequest(HttpContext context)
		{
			if (context.Request.RequestType != "POST")
			{
				context.Response.StatusCode = 403;
				context.Response.StatusDescription = "Forbidden";
				context.Response.End();
				return;
			}

			context.Response.ContentType = "text/plain";

			try
			{
				IGraffitiUser currentUser = GraffitiUsers.Current;
				if (!context.Request.IsAuthenticated || currentUser == null || !GraffitiUsers.IsSiteTeamMember(currentUser))
				{
					throw new SecurityException("Please log in using an administrative account before setting up Graffiti-UserGroups.");
				}

				switch (context.Request.QueryString["command"])
				{
					case "create-event-category":
						CreateCategory<EventPlugin>();
						break;

					case "configure-event-plugin":
						ConfigurePlugin<EventPlugin>();
						break;

					case "enable-event-plugin":
						EnablePlugin<EventPlugin>();
						break;

					case "create-sample-events":
						CreateSampleEvents(15, currentUser);
						break;

					case "create-registration-post":
						CreateRegistrationPost(currentUser);
						break;

					case "create-talk-category":
						//CreateCategory<TalkPlugin>();
						Thread.Sleep(1000);
						break;

					case "configure-talk-plugin":
						//ConfigurePlugin<TalkPlugin>();
						Thread.Sleep(1000);
						break;

					case "enable-talk-plugin":
						//EnablePlugin<TalkPlugin>();
						Thread.Sleep(1000);
						break;

					case "create-sample-talks":
						//CreateSampleTalks(15, currentUser);
						Thread.Sleep(1000);
						break;

					case "create-navigation-links":
						Thread.Sleep(1000);
						break;

					default:
						throw new InvalidOperationException(String.Format("Unknown command '{0}'", context.Request.QueryString["command"]));
				}
			}
			catch (Exception ex)
			{
				Log.Error(String.Format("{0}: Could not process request", GetType().Name), ex.ToString());

				context.Response.StatusCode = 500;
				context.Response.StatusDescription = "Internal server error";

				context.Response.Clear();
				context.Response.Write(ex.Message);
			}
		}

		public bool IsReusable
		{
			get { return false; }
		}
		#endregion

		static TPlugin GetPluginWithCurrentSettings<TPlugin>()
			where TPlugin : GraffitiEvent, ICategoryEnabledRepositoryConfiguration, new()
		{
			EventDetails eventDetails = Events.GetEvent(typeof(TPlugin).GetPluginName());
			return eventDetails.Event as TPlugin;
		}

		void CreateCategory<TPlugin>() where TPlugin : GraffitiEvent, ICategoryEnabledRepositoryConfiguration, new()
		{
			TPlugin plugin = GetPluginWithCurrentSettings<TPlugin>();

			if (_categoryRepository.GetCategory(plugin.CategoryName) != null)
			{
				throw new InvalidOperationException(String.Format("The category '{0}' already exists.", plugin.CategoryName));
			}

			Category category = new Category { Name = plugin.CategoryName, ParentId = -1 };
			_categoryRepository.AddCategory(category);
		}

		static void ConfigurePlugin<TPlugin>()
			where TPlugin : GraffitiEvent, ICategoryEnabledRepositoryConfiguration, ISupportsMemento, new()
		{
			TPlugin plugin = GetPluginWithCurrentSettings<TPlugin>();
			IMemento state = plugin.CreateMemento();

			PluginMigrator.MigrateSettings(true, false, state, state);
		}

		static void EnablePlugin<TPlugin>()
		{
			EventDetails eventDetails = Events.GetEvent(typeof(TPlugin).GetPluginName());
			eventDetails.Enabled = true;
			eventDetails.Event.EventEnabled();
			Events.Save(eventDetails);
		}

		void CreateRegistrationPost(IUser user)
		{
			Post post = CreatePost(user);

			_postRepository.Save(post);
		}

		void CreateSampleEvents(int count, IGraffitiUser user)
		{
			EventPlugin eventPlugin = new EventPlugin();
			Category eventCategory = _categoryRepository.GetCategory("Events");

			DateTime startDate = DateTime.Today.AddMonths(-count / 2);

			for (int i = 1; i <= count; i++)
			{
				Post post = CreatePost(user);
				post.Title = String.Format("Sample Event {0}", i);
				post.CategoryId = eventCategory.Id;

				// One event from 9 AM to 6 PM every two months.
				startDate = startDate.AddMonths(2);
				post[eventPlugin.StartDateField] = startDate.AddHours(9).ToString();
				post[eventPlugin.EndDateField] = startDate.AddHours(18).ToString();
				if (i % 2 == 0)
				{
					post[eventPlugin.LocationField] = "Sample location";
					post[eventPlugin.SpeakerField] = "Sample speaker";
					post[eventPlugin.RegistrationNeededField] = "on";
				}

				_postRepository.Save(post);
			}
		}

		static Post CreatePost(IUser user)
		{
			return new Post
			       {
			       	UniqueId = Guid.NewGuid(),
			       	CreatedBy = user.Name,
			       	ModifiedBy = user.Name,
			       	CreatedOn = DateTime.Now,
			       	ModifiedOn = DateTime.Now,
			       	Published = DateTime.Now,
			       	// Uncategorized.
			       	CategoryId = 1,
			       	Title = "Register",
			       	PostBody = String.Empty,
			       	PostStatus = PostStatus.Publish,
			       	ContentType = "text/html",
			       	IsPublished = true
			       };
		}
	}
}