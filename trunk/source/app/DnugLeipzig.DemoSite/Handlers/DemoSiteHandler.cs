using System;
using System.Security;
using System.Threading;
using System.Web;

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
						CreateCategory("Events");
						break;
					case "configure-event-plugin":
						ConfigureEventPlugin();
						break;
					case "enable-event-plugin":
						EnablePlugin(typeof(EventPlugin));
						break;
					case "create-registration-post":
						CreateRegistrationPost(currentUser);
						break;
					case "create-sample-events":
						CreateSampleEvents(15, currentUser);
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

		void CreateCategory(string categoryName)
		{
			if (String.IsNullOrEmpty(categoryName))
			{
				throw new ArgumentOutOfRangeException("categoryName");
			}

			try
			{
				_categoryRepository.DeleteCategory(categoryName);
			}
			catch(NullReferenceException)
			{
			}

			Category category = new Category { Name = categoryName, ParentId = -1 };
			_categoryRepository.AddCategory(category);
		}

		static void ConfigureEventPlugin()
		{
			EventPlugin eventPlugin = new EventPlugin();
			IMemento state = eventPlugin.CreateMemento();

			EventPlugin.Migrate(true, false, state, state);
		}

		static void EnablePlugin(Type plugingType)
		{
			if (plugingType == null)
			{
				throw new ArgumentNullException("plugingType");
			}

			EventDetails eventDetails = Events.GetEvent(plugingType.AssemblyQualifiedName);
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
				if (i %2 == 0)
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