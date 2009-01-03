using System;
using System.Linq;
using System.Web;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Definitions.Configuration;
using DnugLeipzig.Definitions.Configuration.Plugins;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Definitions.Services;
using DnugLeipzig.ForTesting;
using DnugLeipzig.ForTesting.HttpMocks;
using DnugLeipzig.Runtime.Services;

using Graffiti.Core;

using MbUnit.Framework;

using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace DnugLeipzig.Runtime.Tests.Services
{
	public class When_an_event_registration_request_for_two_events_is_received : With_event_registration_service
	{
		protected override void Establish_context()
		{
			base.Establish_context();

			foreach (int eventId in Command.EventsToRegister)
			{
				Repository.Stub(x => x.GetById(eventId))
					.Return(Create.New.Event(Repository.Configuration)
					        	.Id(eventId)
					        	.OrganizedBy("organizer@example.com"));
			}
		}

		protected override IEventRegistrationCommand CreateCommand()
		{
			return Create.New.EventRegistration()
				.RegisterFor(new[] { 10, 42 })
				.WithEmail("foo@bar.com")
				.Build();
		}

		[Test]
		public void It_should_load_the_events()
		{
			Repository.AssertWasCalled(x => x.GetById(0),
			                           o => o.IgnoreArguments()
			                                	.Repeat.Times(Command.EventsToRegister.Count()));
		}

		[Test]
		public void It_should_save_the_events()
		{
			Repository.AssertWasCalled(x => x.Save(null),
			                           o => o.IgnoreArguments()
			                                	.Repeat.Times(Command.EventsToRegister.Count()));
		}

		[Test]
		public void It_should_add_the_attendee_email_to_the_registration_list()
		{
			Repository.AssertWasCalled(x => x.Save(null),
			                           o =>
			                           o.Constraints(
			                           	Is.Matching<Post>(
			                           		p => p[Repository.Configuration.RegistrationListField].Equals(Command.AttendeeEmail)))
			                           	.Repeat.Times(Command.EventsToRegister.Count()));
		}

		[Test]
		public void It_should_send_registration_emails_to_the_event_organizer()
		{
			EmailSender.AssertWasCalled(x => x.Send(null),
			                            o => o.Constraints(Is.Matching<EmailTemplate>(t => t.From.Equals("foo@bar.com") &&
			                                                                               t.To.Equals("organizer@example.com")))
			                                 	.Repeat.Times(Command.EventsToRegister.Count()));
		}

		[Test]
		public void It_should_return_a_result()
		{
			Assert.IsNotNull(Result);
		}

		[Test]
		public void It_should_be_able_to_render_the_result()
		{
			Result.Render(HttpContext.Current.Response);
		}

		[Test]
		public void It_should_return_a_result_for_each_registered_event()
		{
			Assert.AreEqual(2, Result.Count);
		}

		[Test]
		public void It_should_indicate_that_the_registration_was_successful()
		{
			foreach (var result in Result)
			{
				Assert.IsFalse(result.ErrorOccurred);
			}
		}
	}

	public class When_an_event_registration_request_for_two_events_is_received_and_registration_fails_for_the_first_event
		: With_event_registration_service
	{
		protected override void Establish_context()
		{
			base.Establish_context();

			Repository.Stub(x => x.GetById(10)).Throw(new Exception());

			Repository.Stub(x => x.GetById(42))
				.Return(Create.New.Event(Repository.Configuration)
				        	.Id(42)
				        	.OrganizedBy("organizer@example.com"));
		}

		protected override IEventRegistrationCommand CreateCommand()
		{
			return Create.New.EventRegistration()
				.RegisterFor(new[] { 10, 42 })
				.WithEmail("foo@bar.com")
				.Build();
		}

		[Test]
		public void It_should_save_the_event()
		{
			Repository.AssertWasCalled(x => x.Save(null),
			                           o => o.Constraints(Is.Matching<Post>(p => p.Id.Equals(42))));
		}

		[Test]
		public void It_should_send_registration_emails_to_the_event_organizer()
		{
			EmailSender.AssertWasCalled(x => x.Send(null),
			                            o => o.Constraints(Is.Matching<EmailTemplate>(t => t.From.Equals("foo@bar.com") &&
			                                                                               t.To.Equals("organizer@example.com"))));
		}

		[Test]
		public void It_should_return_a_result()
		{
			Assert.IsNotNull(Result);
		}

		[Test]
		public void It_should_be_able_to_render_the_result()
		{
			Result.Render(HttpContext.Current.Response);
		}

		[Test]
		public void It_should_indicate_that_the_first_registration_had_an_error()
		{
			Assert.IsTrue(Result[0].ErrorOccurred);
		}

		[Test]
		public void It_should_indicate_that_the_second_registration_was_successful()
		{
			Assert.IsFalse(Result[1].ErrorOccurred);
		}
	}

	public class When_an_event_registration_request_and_the_event_is_booked_up : With_event_registration_service
	{
		protected override void Establish_context()
		{
			base.Establish_context();

			foreach (int eventId in Command.EventsToRegister)
			{
				Repository.Stub(x => x.GetById(eventId))
					.Return(Create.New.Event(Repository.Configuration)
					        	.Id(eventId)
					        	.BookedUpWith("2")
					        	.WithAttendeeList("foo@example.com\r\nbar@example.com"));
			}
		}

		protected override IEventRegistrationCommand CreateCommand()
		{
			return Create.New.EventRegistration()
				.RegisterFor(new[] { 42 })
				.WithEmail("foo@bar.com")
				.Build();
		}

		[Test]
		public void It_should_save_the_event()
		{
			Repository.AssertWasCalled(x => x.Save(null),
			                           o =>
			                           o.Constraints(
			                           	Is.Matching<Post>(p => p[Repository.Configuration.RegistrationListField]
			                           	                       	.Contains("foo@bar.com"))));
		}

		[Test]
		public void It_should_indicate_that_the_attendee_is_on_the_waiting_list()
		{
			Assert.IsTrue(Result[0].OnWaitingList);
		}
	}

	public class When_an_event_registration_request_for_two_events_is_received_and_sending_the_emails_fails
		: With_event_registration_service
	{
		protected override void Establish_context()
		{
			base.Establish_context();

			foreach (int eventId in Command.EventsToRegister)
			{
				Repository.Stub(x => x.GetById(eventId))
					.Return(Create.New.Event(Repository.Configuration)
					        	.Id(eventId)
					        	.OrganizedBy("organizer@example.com"));
			}

			EmailSender.Stub(x => x.Send(null)).IgnoreArguments().Throw(new Exception());
		}

		protected override IEventRegistrationCommand CreateCommand()
		{
			return Create.New.EventRegistration()
				.RegisterFor(new[] { 10, 42 })
				.WithEmail("foo@bar.com")
				.Build();
		}

		[Test]
		public void It_should_return_a_result()
		{
			Assert.IsNotNull(Result);
		}

		[Test]
		public void It_should_be_able_to_render_the_result()
		{
			Result.Render(HttpContext.Current.Response);
		}

		[Test]
		public void It_should_return_a_result_for_each_registered_event()
		{
			Assert.AreEqual(2, Result.Count);
		}

		[Test]
		public void It_should_indicate_that_the_registration_was_successful()
		{
			foreach (var result in Result)
			{
				Assert.IsFalse(result.ErrorOccurred);
			}
		}
	}

	public class
		When_an_event_registration_request_for_two_events_is_received_and_the_attendee_wants_to_receive_confirmations
		: With_event_registration_service
	{
		protected override void Establish_context()
		{
			base.Establish_context();

			foreach (int eventId in Command.EventsToRegister)
			{
				Repository.Stub(x => x.GetById(eventId))
					.Return(Create.New.Event(Repository.Configuration)
					        	.Id(eventId));
			}
		}

		protected override IEventRegistrationCommand CreateCommand()
		{
			return Create.New.EventRegistration()
				.RegisterFor(new[] { 10, 42 })
				.WithEmail("foo@bar.com")
				.SendConfirmationToAttendee()
				.Build();
		}

		[Test]
		public void It_should_send_registration_emails_to_the_attendee()
		{
			EmailSender.AssertWasCalled(x => x.Send(null),
			                            o => o.Constraints(Is.Matching<EmailTemplate>(t => t.From == null &&
			                                                                               t.To.Equals("foo@bar.com")))
			                                 	.Repeat.Times(Command.EventsToRegister.Count()));
		}

		[Test]
		public void It_should_indicate_that_the_email_is_sent_to_the_attendee()
		{
			EmailContext.AssertWasCalled(x => x.Put("isCcToAttendee", true),
			                             o => o.Repeat.Times(Command.EventsToRegister.Count()));
		}
	}

	public class
		When_an_event_registration_request_is_received_and_the_registration_list_has_excess_whitespace
		: With_event_registration_service
	{
		protected override void Establish_context()
		{
			base.Establish_context();

			foreach (int eventId in Command.EventsToRegister)
			{
				Repository.Stub(x => x.GetById(eventId))
					.Return(Create.New.Event(Repository.Configuration)
					        	.Id(eventId)
					        	.WithAttendeeList("\n    \r\nblah@example.com\r\n    \r  foobar@baz.com  \n"));
			}
		}

		protected override IEventRegistrationCommand CreateCommand()
		{
			return Create.New.EventRegistration()
				.RegisterFor(new[] { 10 })
				.WithEmail("foo@bar.com")
				.Build();
		}

		[Test]
		public void It_should_save_the_registration_list_in_a_sanitized_format()
		{
			Repository.AssertWasCalled(x => x.Save(null),
			                           o => o.Constraints(Is.Matching<Post>(
			                                              	p => p[Repository.Configuration.RegistrationListField]
			                                              	     	.Equals("blah@example.com\r\nfoobar@baz.com\r\nfoo@bar.com"))));
		}
	}

	public class
		When_an_event_registration_request_is_received_and_the_registration_list_already_contains_the_attendees_email
		: With_event_registration_service
	{
		protected override void Establish_context()
		{
			base.Establish_context();

			foreach (int eventId in Command.EventsToRegister)
			{
				Repository.Stub(x => x.GetById(eventId))
					.Return(Create.New.Event(Repository.Configuration)
					        	.Id(eventId)
					        	.WithAttendeeList("blah@example.com\r\n the-attendee@example.com "));
			}
		}

		protected override IEventRegistrationCommand CreateCommand()
		{
			return Create.New.EventRegistration()
				.RegisterFor(new[] { 10 })
				.WithEmail("the-attendee@example.com")
				.Build();
		}

		[Test]
		public void It_should_not_save_the_post()
		{
			Repository.AssertWasNotCalled(x => x.Save(null), o => o.IgnoreArguments());
		}

		[Test]
		public void It_should_be_able_to_render_the_result()
		{
			Result.Render(HttpContext.Current.Response);
		}

		[Test]
		public void It_should_return_a_result_one_registered_event()
		{
			Assert.AreEqual(1, Result.Count);
		}

		[Test]
		public void It_should_indicate_that_the_registration_was_duplicated()
		{
			foreach (var result in Result)
			{
				Assert.IsTrue(result.AlreadyRegistered);
			}
		}
	}

	public abstract class With_event_registration_service : Spec
	{
		HttpSimulator _request;
		EventRegistrationService _sut;

		protected IGraffitiEmailContext EmailContext
		{
			get;
			private set;
		}

		protected IEventRegistrationCommand Command
		{
			get;
			private set;
		}

		protected ICategorizedPostRepository<IEventPluginConfigurationProvider> Repository
		{
			get;
			private set;
		}

		protected IEventRegistrationResultList Result
		{
			get;
			private set;
		}

		protected IEmailSender EmailSender
		{
			get;
			private set;
		}

		protected override void Establish_context()
		{
			Repository = MockRepository.GenerateMock<ICategorizedPostRepository<IEventPluginConfigurationProvider>>();
			Repository.Stub(x => x.Configuration).Return(Create.New.StubbedEventPluginConfiguration().Build());

			EmailSender = MockRepository.GenerateMock<IEmailSender>();

			EmailContext = MockRepository.GenerateMock<IGraffitiEmailContext>();
			_sut = new EventRegistrationService(Repository,
			                                    EmailContext,
			                                    EmailSender,
			                                    "template.view");

			Command = CreateCommand();

			_request = new HttpSimulator().SimulateRequest();
		}

		protected override void Cleanup_after()
		{
			_request.Dispose();
		}

		protected abstract IEventRegistrationCommand CreateCommand();

		protected override void Because()
		{
			Result = _sut.RegisterForEvents(Command);
		}
	}
}