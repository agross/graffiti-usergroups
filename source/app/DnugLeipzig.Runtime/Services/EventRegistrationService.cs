using System;
using System.Text.RegularExpressions;
using System.Web;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Definitions.Extensions;
using DnugLeipzig.Definitions.GraffitiIntegration;
using DnugLeipzig.Definitions.Plugins.Events;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Definitions.Services;
using DnugLeipzig.Runtime.Macros;
using DnugLeipzig.Runtime.Macros.Extensions;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Services
{
	public class EventRegistrationService : Service, IEventRegistrationService
	{
		static readonly object PostLock = new object();

		/// <summary>
		///  Beginning of line or string
		///  Match expression but don't capture it. [\s*]
		///      Whitespace, any number of repetitions
		///  [Email]: A named capture group. [.+?]
		///      Any character, one or more repetitions, as few as possible
		///  Match expression but don't capture it. [\s*]
		///      Whitespace, any number of repetitions
		///  End of line or string
		/// </summary>
		public static Regex EmailLines = new Regex("^(?:\\s*)(?<Email>.+?)(?:\\s*)$",
		                                           RegexOptions.IgnoreCase
		                                           | RegexOptions.Multiline
		                                           | RegexOptions.Singleline
		                                           | RegexOptions.ExplicitCapture
		                                           | RegexOptions.CultureInvariant
		                                           | RegexOptions.IgnorePatternWhitespace
		                                           | RegexOptions.Compiled);

		readonly IClock _clock;

		readonly IGraffitiEmailContext _emailContext;
		readonly IEmailSender _emailSender;
		readonly string _registrationEmailTemplate;
		readonly ICategorizedPostRepository<IEventPluginConfigurationProvider> _repository;

		public EventRegistrationService(ICategorizedPostRepository<IEventPluginConfigurationProvider> repository,
		                                IGraffitiEmailContext emailContext,
		                                IEmailSender emailSender,
		                                string registrationEmailTemplate,
		                                IClock clock)
		{
			_repository = repository;
			_emailContext = emailContext;
			_emailSender = emailSender;
			_registrationEmailTemplate = registrationEmailTemplate;
			_clock = clock;
		}

		#region Implementation of IEventRegistrationService
		public IEventRegistrationResultList RegisterForEvents(IEventRegistrationCommand command)
		{
			EventRegistrationResultList results = new EventRegistrationResultList();

			foreach (int eventId in command.EventsToRegister)
			{
				try
				{
					var result = RegisterAttendeeForEvent(eventId, command);
					results.Add(result);

					SendEmail(PrepareEmailForEventOrganizer(result, command, null));
					SendEmail(PrepareEmailForAttendee(result, command));
				}
				catch (Exception ex)
				{
					Logger.Error(Create.New.LogMessage().WithTitle("Could not process registration for event {0}", eventId), ex);
				}
			}

			return results;
		}
		#endregion

		void SendEmail(EmailTemplate template)
		{
			if (template != null)
			{
				_emailSender.Send(template);
			}
		}

		EmailTemplate PrepareEmailForEventOrganizer(EventRegistrationResult result,
		                                            IEventRegistrationCommand command,
		                                            Action<IGraffitiEmailContext> alterations)
		{
			IGraffitiEmailContext context = _emailContext;
			context.Put("request", HttpContext.Current.Request);
			context.Put("events", new EventMacros());
			context.Put("name", HttpUtility.HtmlEncode(command.Name));
			context.Put("attendeeEMail", HttpUtility.HtmlEncode(command.AttendeeEmail));
			context.Put("event", result.Post);
			context.Put("isCcToAttendee", false);
			context.Put("isOnWaitingList", result.OnWaitingList);
			context.Put("isAlreadyRegistered", result.AlreadyRegistered);

			if (alterations != null)
			{
				alterations(context);
			}

			return new EmailTemplate
			       {
			       	Subject = _repository.Configuration.RegistrationMailSubject,
			       	Context = context.ToEmailTemplateToolboxContext(),
			       	TemplateName = _registrationEmailTemplate,
			       	From = null,
			       	To = result.Post[_repository.Configuration.RegistrationRecipientField]
			       };
		}

		EmailTemplate PrepareEmailForAttendee(EventRegistrationResult result, IEventRegistrationCommand command)
		{
			if (!command.SendConfirmationToAttendee)
			{
				return null;
			}

			EmailTemplate template = PrepareEmailForEventOrganizer(result,
			                                                       command,
			                                                       context => context.Put("isCcToAttendee", true));

			// Send from the site's e-mail address.
			template.From = null;
			template.To = command.AttendeeEmail;

			return template;
		}

		EventRegistrationResult RegisterAttendeeForEvent(int eventId, IEventRegistrationCommand command)
		{
			try
			{
				Logger.Info(Create.New.LogMessage()
				            	.WithTitle("Event registration received")
				            	.WithMessage("From: {0}", command.AttendeeEmail));

				Post post = _repository.GetById(eventId);

				Logger.Info(Create.New.LogMessage()
				            	.WithTitle("Processing event registration for event {0}", eventId)
				            	.WithMessage("From: {0}, Recipient: {1}",
				            	             command.AttendeeEmail,
				            	             post[_repository.Configuration.RegistrationRecipientField]));

				// Only a single thread can work with posts such that two threads don't mess with the number of received 
				// registrations. This does not prevent the CMS itself to work with the post, though.
				lock (PostLock)
				{
					if (!post.RegistrationPossible(_repository.Configuration.RegistrationListField,
					                               _repository.Configuration.MaximumNumberOfRegistrationsField,
					                               _repository.Configuration.EarliestRegistrationField,
					                               _repository.Configuration.LatestRegistrationField,
					                               _repository.Configuration.StartDateField,
												   _clock))
					{
						Logger.Warn(Create.New
						            	.LogMessage()
						            	.WithTitle(
						            	"Received a registration for an event for which registration has been disabled. Event ID {0}",
						            	eventId));
						return EventRegistrationResult.NotAllowedFor(post);
					}

					var entries = EmailLines.Matches(post[_repository.Configuration.RegistrationListField] ?? String.Empty);

					string[] lines = new string[entries.Count + 1];
					for (int i = 0; i < entries.Count; i++)
					{
						lines[i] = entries[i].Groups["Email"].Value;
					}

					if (Array.Exists(lines, l => l != null && l.Equals(command.AttendeeEmail, StringComparison.OrdinalIgnoreCase)))
					{
						return EventRegistrationResult.AlreadyRegisteredFor(post);
					}

					lines[entries.Count] = command.AttendeeEmail;

					post[_repository.Configuration.RegistrationListField] = String.Join(Environment.NewLine, lines);

					int numberOfRegistations = lines.Length;
					int maximumNumberOfRegistations = post[_repository.Configuration.MaximumNumberOfRegistrationsField]
						.ToInt(int.MaxValue);

					_repository.Save(post);

					return EventRegistrationResult.SuccessfullyRegisteredFor(post, numberOfRegistations > maximumNumberOfRegistations);
				}
			}
			catch (Exception ex)
			{
				Logger.Error(Create.New.LogMessage().WithTitle("Could not process registration for event ID {0}", eventId), ex);
				return EventRegistrationResult.Error();
			}
		}
	}
}