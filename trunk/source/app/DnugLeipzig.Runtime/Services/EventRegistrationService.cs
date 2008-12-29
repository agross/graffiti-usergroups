using System;

using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Definitions.Commands.Events;
using DnugLeipzig.Definitions.Configuration;
using DnugLeipzig.Definitions.Configuration.Plugins;
using DnugLeipzig.Definitions.Extensions;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Definitions.Services;

using Graffiti.Core;

using NVelocity.Context;

namespace DnugLeipzig.Runtime.Services
{
	public class EventRegistrationService : Service, IEventRegistrationService
	{
		static readonly object PostLock = new object();
		readonly IEmailSender _emailSender;
		readonly string _registrationEmailTemplate;
		readonly ICategorizedPostRepository<IEventPluginConfiguration> _repository;
		readonly IGraffitiSettings _settings;

		public EventRegistrationService(ICategorizedPostRepository<IEventPluginConfiguration> repository,
		                                IEmailSender emailSender,
		                                IGraffitiSettings settings,
		                                string registrationEmailTemplate)
		{
			_repository = repository;
			_emailSender = emailSender;
			_settings = settings;
			_registrationEmailTemplate = registrationEmailTemplate;
		}

		#region Implementation of IEventRegistrationService
		public ICommandResult RegisterForEvents(MultipleEventRegistrationCommand command)
		{
			MultipleEventRegistrationResult result = new MultipleEventRegistrationResult();

			foreach (var eventToRegister in command.EventsToRegister)
			{
				result.EventResults.Add(
					RegisterForEvent(new SingleEventRegistrationCommand(eventToRegister,
					                                                    command.Name,
					                                                    command.FormOfAddress,
					                                                    command.Occupation,
					                                                    command.AttendeeEmail,
					                                                    command.SendConfirmationToAttendee)));
			}

			return result;
		}

		public ICommandResult RegisterForEvent(SingleEventRegistrationCommand command)
		{
			try
			{
				throw new NotImplementedException();
				//				ICollection<string> validationErrors = command.Validate();
				//				if (validationErrors.Count != 0)
				//				{
				//					return new ValidationErrorResult(validationErrors);
				//				}
				//
				//				Logger.Info("Event registration received|From: {0}", command.AttendeeEmail);
				//
				//				EmailTemplateToolboxContext mailContext = new EmailTemplateToolboxContext();
				//				mailContext.Put("request", context.Request);
				//				mailContext.Put("events", new EventMacros());
				//				mailContext.Put("formOfAddress", HttpUtility.HtmlEncode(command.FormOfAddress));
				//				mailContext.Put("name", HttpUtility.HtmlEncode(command.Name));
				//				mailContext.Put("occupation", HttpUtility.HtmlEncode(command.Occupation));
				//				mailContext.Put("attendeeEMail", HttpUtility.HtmlEncode(command.AttendeeEmail));
				//
				//				EmailTemplate emailTemplate = new EmailTemplate
				//				                              {
				//				                              	Subject = _configuration.RegistrationMailSubject,
				//				                              	Context = mailContext,
				//				                              	TemplateName = _registrationEmailTemplate
				//				                              };
				//
				//				SingleEventRegistrationResult result = new SingleEventRegistrationResult(command.EventToRegister);
				//
				//				// Only a single thread can work with posts such that two threads don't mess with the number of received registrations.
				//				lock (PostLock)
				//				{
				//					Post eventToRegister = _repository.GetById(command.EventToRegister);
				//
				//					Logger.Info("Processing event registration|From: {0}, Recipient: {1}, Event: {2}",
				//					            command.AttendeeEmail,
				//					            eventToRegister[_configuration.RegistrationRecipientField],
				//					            _settings.Site.BaseUrl + eventToRegister.Url);
				//
				//					result.IsOnWaitingList = IsOnWaitingList(eventToRegister, command);
				//
				//					PrepareEmail(mailContext, emailTemplate, eventToRegister, command, result.IsOnWaitingList, false);
				//					_emailSender.Send(emailTemplate);
				//
				//					if (command.SendConfirmationToAttendee)
				//					{
				//						PrepareEmail(mailContext, emailTemplate, eventToRegister, command, result.IsOnWaitingList, true);
				//						_emailSender.Send(emailTemplate);
				//					}
				//
				//					_repository.Save(eventToRegister);
				//				}
				//
				//				return result;
			}
			catch (Exception ex)
			{
				Logger.Error("Could not process registration", ex.ToString());
				throw;
			}
		}
		#endregion

		protected virtual bool IsOnWaitingList(Post post, SingleEventRegistrationCommand command)
		{
			post[_repository.Configuration.RegistrationListField] += command.AttendeeEmail + Environment.NewLine;

			int numberOfRegistations = post[_repository.Configuration.RegistrationListField].LineCount();
			int maximumNumberOfRegistations =
				post[_repository.Configuration.MaximumNumberOfRegistrationsField].ToInt(int.MaxValue);

			return numberOfRegistations > maximumNumberOfRegistations;
		}

		void PrepareEmail(IContext mailContext,
		                  EmailTemplate emailTemplate,
		                  Post eventToRegister,
		                  EventRegistrationCommand request,
		                  bool isOnWaitingList,
		                  bool isCcToAttendee)
		{
			mailContext.Put("event", eventToRegister);
			mailContext.Put("isCcToAttendee", isCcToAttendee);
			mailContext.Put("isOnWaitingList", isOnWaitingList);

			emailTemplate.From = request.AttendeeEmail;
			emailTemplate.To = eventToRegister[_repository.Configuration.RegistrationRecipientField];

			if (isCcToAttendee)
			{
				// Send from site's e-mail address.
				emailTemplate.From = null;
				emailTemplate.To = request.AttendeeEmail;
			}
		}

		/*
			public ICollection<string> Validate()
			{
				// TODO: English.
				List<string> validationErrors = new List<string>();
				if (!Validator.ValidateRange(RegisteredEvents.Count, 1, int.MaxValue))
				{
					validationErrors.Add("Please select at least one event to register for.");
				}

				if (!Validator.ValidateExisting(FormOfAddress))
				{
					validationErrors.Add("Please select a form of address.");
				}

				if (!Validator.ValidateExisting(Name))
				{
					validationErrors.Add("Please enter your name.");
				}

				if (!Validator.ValidateExisting(Occupation))
				{
					validationErrors.Add("Please select your occupation.");
				}

				if (!Validator.ValidateEmail(AttendeeEMail))
				{
					validationErrors.Add("Please enter your e-mail address.");
				}

				return validationErrors;
			}
		}
*/
	}
}