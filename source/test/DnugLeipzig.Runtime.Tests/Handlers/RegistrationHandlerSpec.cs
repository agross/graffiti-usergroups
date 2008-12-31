using System;
using System.Collections.Specialized;
using System.Web;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.ForTesting;
using DnugLeipzig.ForTesting.HttpMocks;
using DnugLeipzig.Runtime.Handlers;

using MbUnit.Framework;

using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace DnugLeipzig.Runtime.Tests.Handlers
{
	public class When_the_registration_handler_is_called_via_Http_get : With_registration_handler
	{
		protected override HttpSimulator CreateRequest()
		{
			return new HttpSimulator().SimulateRequest(new Uri("http://foo"), HttpVerb.GET);
		}

		[Test]
		public void It_should_set_Http_403_status()
		{
			Assert.AreEqual(403, HttpContext.Current.Response.StatusCode);
		}
	}

	public class When_the_registration_handler_is_called_with_an_unknown_command : With_registration_handler
	{
		protected override HttpSimulator CreateRequest()
		{
			return new HttpSimulator().SimulateRequest(new Uri(String.Format("http://foo?command={0}",
			                                                                 Guid.NewGuid())),
			                                           HttpVerb.POST);
		}

		[Test]
		public void It_should_set_Http_500_status()
		{
			Assert.AreEqual(500, HttpContext.Current.Response.StatusCode);
		}
	}

	public class When_the_registration_handler_is_called_with_the_register_command : With_registration_handler
	{
		IEventRegistrationCommand _command;
		IHttpResponse _result;

		protected override void Establish_context()
		{
			base.Establish_context();

			_command = MockRepository.GenerateMock<IEventRegistrationCommand>();
			CommandFactory.Stub(x => x.EventRegistration(null, null, null, null, null, true))
				.IgnoreArguments()
				.Return(_command);

			_result = MockRepository.GenerateMock<IHttpResponse>();
			_command.Stub(x => x.Execute()).Return(_result);
		}

		protected override HttpSimulator CreateRequest()
		{
			var form = new NameValueCollection
			           {
			           	{ "event-10", null },
			           	{ "event-42", null },
			           	{ "formOfAddress", "form of address" },
			           	{ "name", "firstname lastname" },
			           	{ "occupation", "IANAL" },
			           	{ "attendeeEMail", "foo@bar.com" },
			           	{ "ccToAttendee", "on" }
			           };

			return new HttpSimulator().SimulateRequest(new Uri("http://foo?command=register"), form);
		}

		[Test]
		public void It_should_create_a_registration_request_from_the_form_values()
		{
			CommandFactory.AssertWasCalled(x => x.EventRegistration(null, null, null, null, null, true),
			                               x => x.Constraints(List.ContainsAll(new[] { 10, 42 }),
															  Is.Equal("firstname lastname"),
															  Is.Equal("form of address"),
															  Is.Equal("IANAL"),
			                                                  Is.Equal("foo@bar.com"),
			                                                  Is.Equal(true)));
		}

		[Test]
		public void It_should_execute_the_request()
		{
			_command.AssertWasCalled(x => x.Execute());
		}

		[Test]
		public void It_should_render_the_request_result()
		{
			_result.AssertWasCalled(x => x.Render(null), x => x.IgnoreArguments());
		}

		[Test]
		public void It_should_set_Http_200_status()
		{
			Assert.AreEqual(200, HttpContext.Current.Response.StatusCode);
		}
	}

	public abstract class With_registration_handler : Spec
	{
		RegistrationHandler _sut;

		protected ICommandFactory CommandFactory
		{
			get;
			private set;
		}

		protected HttpSimulator Request
		{
			get;
			private set;
		}

		protected override void Establish_context()
		{
			CommandFactory = MockRepository.GenerateStub<ICommandFactory>();
			_sut = new RegistrationHandler(CommandFactory, null);

			Request = CreateRequest();
		}

		protected override void Because()
		{
			_sut.ProcessRequest(HttpContext.Current);
		}

		protected override void Cleanup_after()
		{
			Request.Dispose();
		}

		protected abstract HttpSimulator CreateRequest();
	}
}