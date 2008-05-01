using System;
using System.Collections.Generic;
using System.Web;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Configuration;
using DnugLeipzig.Definitions.Extensions;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Extensions.Configuration;
using DnugLeipzig.Extensions.Macros;
using DnugLeipzig.Runtime;
using DnugLeipzig.Runtime.Repositories;

using Graffiti.Core;

using NVelocity.Context;

namespace DnugLeipzig.Extensions.Handlers
{
	public class RegistrationHandler : IHttpHandler
	{
		static readonly object _postLock = new object();
		readonly IEventPluginConfiguration _configuration;
		readonly ICategoryEnabledRepository _repository;
		readonly IEmailSender _emailSender;

		#region Ctors
		/// <summary>
		/// Initializes a new instance of the <see cref="RegistrationHandler"/> class.
		/// </summary>
		public RegistrationHandler() : this(null, new EventPluginConfiguration(), new GraffitiEmailSender())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RegistrationHandler"/> class.
		/// This constructor is used for dependency injection in unit testing scenarios.
		/// </summary>
		/// <param name="repository">The repository.</param>
		/// <param name="configuration">The configuration.</param>
		/// <param name="emailSender">The email sender.</param>
		public RegistrationHandler(ICategoryEnabledRepository repository, IEventPluginConfiguration configuration, IEmailSender emailSender)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}

			_configuration = configuration;

			if (repository == null)
			{
				repository = new EventRepository(_configuration);
			}

			_repository = repository;

			if (emailSender == null)
			{
				throw new ArgumentNullException("emailSender");
			}

			_emailSender = emailSender;
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

			try
			{
				switch (context.Request.QueryString["command"])
				{
					case "register":
						RegistrationResponse response = ProcessRegistration(context);
						context.Response.ContentType = "application/json";
						context.Response.Write(response.ToJson());
						break;

					default:
						throw new HttpException(500, String.Format("Unknown command '{0}'", context.Request.QueryString["command"]));
				}
			}
			catch (Exception ex)
			{
				Log.Error(String.Format("{0}: Could not process request", GetType().Name), ex.ToString());

				context.Response.ContentType = "text/plain";
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

		RegistrationResponse ProcessRegistration(HttpContext context)
		{
			try
			{
				string[] events = Array.FindAll(context.Request.Form.AllKeys, key => key.StartsWith("event-"));

				RegistrationRequest request = new RegistrationRequest
				                              {
				                              	RegisteredEvents =
				                              		Array.ConvertAll(events, e => Convert.ToInt32(e.Replace("event-", String.Empty))),
				                              	FormOfAddress = context.Request.Form["formOfAddress"],
				                              	Name = context.Request.Form["name"],
				                              	Occupation = context.Request.Form["occupation"],
				                              	AttendeeEMail = context.Request.Form["attendeeEMail"],
				                              	CcToAttendee = context.Request.Form["ccToAttendee"].IsChecked()
				                              };

				ICollection<string> validationErrors = request.Validate();
				if (validationErrors.Count != 0)
				{
					return new RegistrationResponse { ValidationErrors = validationErrors };
				}

				Log.Info("Event registration received", String.Format("Sender: {0}", request.AttendeeEMail));

				RegistrationResponse response = new RegistrationResponse();

				EmailTemplateToolboxContext mailContext = new EmailTemplateToolboxContext();
				mailContext.Put("request", context.Request);
				mailContext.Put("events", new EventMacros());
				mailContext.Put("formOfAddress", context.Server.HtmlEncode(request.FormOfAddress));
				mailContext.Put("name", context.Server.HtmlEncode(request.Name));
				mailContext.Put("occupation", context.Server.HtmlEncode(request.Occupation));
				mailContext.Put("attendeeEMail", context.Server.HtmlEncode(request.AttendeeEMail));

				EmailTemplate emailTemplate = new EmailTemplate
				                              {
				                              	Subject = _configuration.RegistrationMailSubject,
				                              	Context = mailContext,
				                              	TemplateName = "register.view"
				                              };

				// Only a single thread can work with posts such that two threads don't mess with the number of received registrations.
				lock (_postLock)
				{
					foreach (int eventId in request.RegisteredEvents)
					{
						Post post = _repository.GetById(eventId);

						bool isOnWaitingList = ProcessSingleRegistration(post);
						if(isOnWaitingList)
						{
							response.WaitingListEvents.Add(eventId);
						}

						PrepareEmail(mailContext, emailTemplate, post, request, isOnWaitingList, false);
						SendEmail(emailTemplate);

						if (request.CcToAttendee)
						{
							PrepareEmail(mailContext, emailTemplate, post, request, isOnWaitingList, true);
							SendEmail(emailTemplate);
						}

						_repository.Save(post);
					}
				}

				response.Success = true;
				return response;
			}
			catch (Exception ex)
			{
				Log.Error("Could not process registration", ex.ToString());
				throw;
			}
		}

		bool ProcessSingleRegistration(Post post)
		{
			int numberOfRegistations = post[_configuration.NumberOfRegistrationsField].ToInt(0);
			int maximumNumberOfRegistations = post[_configuration.MaximumNumberOfRegistrationsField].ToInt(int.MaxValue);

			// Check for overflows (very unlikely).
			checked
			{
				post[_configuration.NumberOfRegistrationsField] = (++numberOfRegistations).ToString();
			}

			return numberOfRegistations > maximumNumberOfRegistations;
		}

		void PrepareEmail(IContext mailContext,
		                  EmailTemplate emailTemplate,
		                  Post post,
		                  RegistrationRequest request,
		                  bool isOnWaitingList,
		                  bool isCcToAttendee)
		{
			mailContext.Put("event", post);
			mailContext.Put("isCcToAttendee", isCcToAttendee);
			mailContext.Put("isOnWaitingList", isOnWaitingList);

			emailTemplate.From = request.AttendeeEMail;
			emailTemplate.To = post[_configuration.RegistrationRecipientField];

			if (request.CcToAttendee)
			{
				emailTemplate.From = null;
				emailTemplate.To = request.AttendeeEMail;
			}
		}

		void SendEmail(EmailTemplate emailTemplate)
		{
			try
			{
				_emailSender.Send(emailTemplate);
			}
			catch (Exception ex)
			{
				Log.Error("Could not send registration message", ex.ToString());
				throw;
			}
		}
	}
}