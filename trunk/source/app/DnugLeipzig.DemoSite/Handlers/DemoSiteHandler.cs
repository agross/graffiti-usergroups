using System;
using System.Security;
using System.Web;

using DnugLeipzig.Definitions.Extensions;
using DnugLeipzig.Definitions.Plugins;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Runtime.Plugins.Events;
using DnugLeipzig.Runtime.Plugins.Migration;
using DnugLeipzig.Runtime.Plugins.Talks;
using DnugLeipzig.Runtime.Repositories;

using Graffiti.Core;

namespace DnugLeipzig.DemoSite.Handlers
{
	public class DemoSiteHandler : IHttpHandler
	{
		const string RegisterPostTitle = "Register";
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
				if (!context.Request.IsAuthenticated || currentUser == null || !GraffitiUsers.IsAdmin(currentUser))
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
						CreateSampleEvents(10, currentUser);
						break;

					case "create-registration-post":
						CreateRegistrationPost(currentUser);
						break;

					case "create-talk-category":
						CreateCategory<TalkPlugin>();
						break;

					case "configure-talk-plugin":
						ConfigurePlugin<TalkPlugin>();
						break;

					case "enable-talk-plugin":
						EnablePlugin<TalkPlugin>();
						break;

					case "create-sample-talks":
						CreateSampleTalks(10, currentUser);
						break;

					case "create-navigation-links":
						CreateNavigationLink<EventPlugin>();
						CreateNavigationLink<TalkPlugin>();
						CreateNavigationLink(RegisterPostTitle);
						break;
		
					case "load-navigation":
						context.Response.Write(RenderNavigation());
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

		static string RenderNavigation()
		{
			Graffiti.Core.Macros macros = new Graffiti.Core.Macros();
			return macros.LoadThemeView("components/site/menu.view");
		}

		void CreateCategory<TPlugin>() where TPlugin : GraffitiEvent, IPluginConfigurationProvider
		{
			TPlugin plugin = PluginHelper.GetPluginWithCurrentSettings<TPlugin>();

			if (_categoryRepository.GetCategory(plugin.CategoryName) != null)
			{
				throw new InvalidOperationException(String.Format("The category '{0}' already exists.", plugin.CategoryName));
			}

			Category category = new Category { Name = plugin.CategoryName, ParentId = -1 };
			_categoryRepository.AddCategory(category);
		}

		static void ConfigurePlugin<TPlugin>()
			where TPlugin : GraffitiEvent, ISupportsMemento
		{
			TPlugin plugin = PluginHelper.GetPluginWithCurrentSettings<TPlugin>();
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
			post.Title = RegisterPostTitle;
			// Uncategorized.
			post.CategoryId = 1;

			_postRepository.Save(post);
		}

		void CreateSampleEvents(int count, IUser user)
		{
			EventPlugin eventPlugin = PluginHelper.GetPluginWithCurrentSettings<EventPlugin>();
			Category eventCategory = _categoryRepository.GetCategory(eventPlugin.CategoryName);

			DateTime startDate = DateTime.Today.AddMonths(-count / 2);

			for (int i = 1; i <= count; i++)
			{
				Post post = CreatePost(user);
				post.Title = String.Format("Sample Event {0}", i);
				post.PostBody = String.Format("<h3>Sample Event {0} Heading</h3><p>Sample Event {0} contents</p>", i);
				post.CategoryId = eventCategory.Id;

				// One event from 9 AM to 6 PM every two months.
				startDate = startDate.AddMonths(2).AddDays(1);
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

		void CreateSampleTalks(int count, IUser user)
		{
			TalkPlugin talkPlugin = PluginHelper.GetPluginWithCurrentSettings<TalkPlugin>(); 
			Category talkCategory = _categoryRepository.GetCategory(talkPlugin.CategoryName);

			DateTime date = DateTime.Today.AddMonths(-count / 2).AddDays(1);

			for (int i = 1; i <= count; i++)
			{
				Post post = CreatePost(user);
				post.Title = String.Format("Sample Talk {0}", i);
				post.PostBody = String.Format("<h3>Sample Talk {0} Heading</h3><p>Sample Talk {0} contents</p>", i);
				post.CategoryId = talkCategory.Id;

				// One talk every two months.
				date = date.AddMonths(2).AddDays(1);
				post[talkPlugin.DateField] = date.ToString();
				post[talkPlugin.SpeakerField] = "Sample speaker";

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
			       	PostBody = String.Empty,
			       	PostStatus = PostStatus.Publish,
			       	ContentType = "text/html",
			       	IsPublished = true,
					
			       };
		}

		void CreateNavigationLink<TPlugin>() where TPlugin : GraffitiEvent, IPluginConfigurationProvider, new()
		{
			TPlugin plugin = PluginHelper.GetPluginWithCurrentSettings<TPlugin>();
			Category category = _categoryRepository.GetCategory(plugin.CategoryName);

			// Dynamic navigation items for categories are automatically created if the dynamic navigation items are empty.
            NavigationSettings settings = NavigationSettings.Get();
			if(settings.SafeItems().Exists(dni => dni.NavigationType == DynamicNavigationType.Category && dni.CategoryId == category.Id))
			{
				// The dynamic navigation item already exists.
				return;
			}
			
			DynamicNavigationItem item = new DynamicNavigationItem
			                             {
			                             	NavigationType = DynamicNavigationType.Category,
			                             	CategoryId = category.Id,
			                             	Id = Guid.NewGuid()
			                             };

			NavigationSettings.Add(item);
		}

		void CreateNavigationLink(string postName)
		{
			Post post = _postRepository.GetByTitle(postName);

			DynamicNavigationItem item = new DynamicNavigationItem
			                             {
			                             	NavigationType = DynamicNavigationType.Post,
			                             	PostId = post.Id,
			                             	Id = Guid.NewGuid()
			                             };

			NavigationSettings.Add(item);
		}
	}
}