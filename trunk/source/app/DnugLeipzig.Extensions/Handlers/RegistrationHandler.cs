using System;
using System.Collections.Generic;
using System.Web;

using DnugLeipzig.Definitions.Configuration;
using DnugLeipzig.Definitions.Extensions;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Extensions.Macros;
using DnugLeipzig.Runtime.Repositories;

using Graffiti.Core;

using NVelocity.Context;

namespace DnugLeipzig.Extensions.Handlers
{
	public class RegistrationHandler : IHttpHandler
	{
		static readonly object _postLock = new object();
		readonly IEventPluginConfiguration Configuration;
		readonly ICategoryEnabledRepository Repository;

		#region Ctors
		public RegistrationHandler() : this(null, new EventPluginConfiguration())
		{
		}

		public RegistrationHandler(ICategoryEnabledRepository repository, IEventPluginConfiguration configuration)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}

			Configuration = configuration;

			if (repository == null)
			{
				repository = new EventRepository(Configuration);
			}

			Repository = repository;
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
				switch (context.Request.QueryString["command"])
				{
					case "register":
						ProcessRegistration(context);
						break;
					default:
						throw new HttpException(500, String.Format("Unknown command '{0}'", context.Request.QueryString["command"]));
				}
			}
			catch (Exception ex)
			{
				Log.Error(String.Format("{0}: Could not process request", GetType().Name), ex.Message);

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

		void ProcessRegistration(HttpContext context)
		{
			// TODO: Form validation

			try
			{
				string[] events = Array.FindAll(context.Request.Form.AllKeys, key => key.StartsWith("event"));
				int[] eventIds = Array.ConvertAll(events, e => Convert.ToInt32(e.Replace("event", String.Empty)));

				string formOfAddress = context.Request.Form["formOfAddress"];
				string name = context.Request.Form["name"];
				string occupation = context.Request.Form["occupation"];
				string attendeeEMail = context.Request.Form["attendeeEMail"];
				bool ccToAttendee = context.Request.Form["ccToAttendee"].IsChecked();

				EmailTemplateToolboxContext mailContext = new EmailTemplateToolboxContext();
				mailContext.Put("events", new EventMacros());
				mailContext.Put("formOfAddress", context.Server.HtmlEncode(formOfAddress));
				mailContext.Put("name", context.Server.HtmlEncode(name));
				mailContext.Put("occupation", context.Server.HtmlEncode(occupation));
				mailContext.Put("attendeeEMail", context.Server.HtmlEncode(attendeeEMail));

				EmailTemplate emailTemplate = new EmailTemplate
				                              {
				                              	Subject = Configuration.RegistrationMailSubject,
				                              	Context = mailContext,
				                              	From = attendeeEMail,
				                              	TemplateName = "register.view"
				                              };

				Log.Info("Event registration received", String.Format("Sender: {0}", attendeeEMail));
				SendEMail(mailContext, emailTemplate, eventIds, attendeeEMail, ccToAttendee);

				context.Response.Write("Vielen Dank für Ihre Anmeldung.");
			}
			catch (Exception ex)
			{
				Log.Error("Could not process registration", ex.Message);
				throw;
			}
		}

		void SendEMail(IContext mailContext,
		               EmailTemplate emailTemplate,
		               IEnumerable<int> eventIds,
		               string attendeeEMail,
		               bool ccToAttendee)
		{
			// Only a single thread can work with posts such that two threads don't mess with the number of received registrations.
			lock (_postLock)
			{
				foreach (int eventId in eventIds)
				{
					Post post = Repository.GetById(eventId);

					int numberOfRegistations = post[Configuration.NumberOfRegistrationsField].ToInt(0);
					int maximumNumberOfRegistations = post[Configuration.MaximumNumberOfRegistrationsField].ToInt(int.MaxValue);

					// Check for overflows (very unlikely).
					checked
					{
						post[Configuration.NumberOfRegistrationsField] = (++numberOfRegistations).ToString();
					}

					mailContext.Put("event", post);
					mailContext.Put("isCcToAttendee", false);
					mailContext.Put("isOnWaitingList", numberOfRegistations > maximumNumberOfRegistations);
					emailTemplate.To = post[Configuration.RegistrationRecipientField];

					try
					{
						MailHelper.Send(emailTemplate);

						if (ccToAttendee)
						{
							mailContext.Put("isCcToAttendee", true);
							emailTemplate.From = null;
							emailTemplate.To = attendeeEMail;
							MailHelper.Send(emailTemplate);
						}
					}
					catch (Exception ex)
					{
						Log.Error("Could not send registration message", ex.Message);
						throw;
					}

					Repository.Save(post);
				}
			}
		}
	}
}