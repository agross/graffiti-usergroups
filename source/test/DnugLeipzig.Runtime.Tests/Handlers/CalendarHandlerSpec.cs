using System;
using System.Collections.Generic;
using System.Web;

using Castle.Core.Logging;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Plugins.Events;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.ForTesting;
using DnugLeipzig.ForTesting.Builders;
using DnugLeipzig.ForTesting.HttpMocks;
using DnugLeipzig.Runtime.Handlers;

using Graffiti.Core;

using MbUnit.Framework;

using Rhino.Mocks;

namespace DnugLeipzig.Runtime.Tests.Handlers
{
	public class When_the_calendar_handler_is_called_via_Http_post : With_calendar_handler
	{
		protected override HttpSimulator CreateRequest()
		{
			return new HttpSimulator().SimulateRequest(new Uri("http://foo"), HttpVerb.POST);
		}

		[Test]
		public void It_should_set_Http_403_status()
		{
			Assert.AreEqual(403, HttpContext.Current.Response.StatusCode);
		}
	}

	public class When_an_event_calendar_item_is_requested_for_a_non_numeric_event_ID : With_calendar_handler
	{
		protected override HttpSimulator CreateRequest()
		{
			return new HttpSimulator().SimulateRequest(new Uri("http://foo?eventId=abc"), HttpVerb.GET);
		}

		[Test]
		public void It_should_set_Http_404_status()
		{
			Assert.AreEqual(404, HttpContext.Current.Response.StatusCode);
		}
	}

	public class When_an_event_calendar_item_is_requested_for_a_non_existing_event : With_calendar_handler
	{
		protected override void Establish_context()
		{
			base.Establish_context();

			PostRepository.Stub(x => x.GetById(42)).Return(null);
		}

		protected override HttpSimulator CreateRequest()
		{
			return new HttpSimulator().SimulateRequest(new Uri("http://foo?eventId=42"), HttpVerb.GET);
		}

		[Test]
		public void It_should_set_Http_404_status()
		{
			Assert.AreEqual(404, HttpContext.Current.Response.StatusCode);
		}
	}

	public class When_an_event_calendar_item_is_requested : With_calendar_handler
	{
		ICalendar _calendar;
		Post _event;

		protected override void Establish_context()
		{
			base.Establish_context();

			_event = Create.New.Event()
				.Id(42)
				.StartingAt(DateTime.MinValue)
				.To(DateTime.MinValue.AddDays(10))
				.AtLocation("somewhere")
				.TheTopicIs("techno babble");
			PostRepository.Stub(x => x.GetById(42)).Return(_event);

			_calendar = MockRepository.GenerateMock<ICalendar>();
			CalendarItemRepository.Stub(x => x.CreateCalendar(_event))
				.Return(_calendar);
		}

		protected override HttpSimulator CreateRequest()
		{
			return new HttpSimulator().SimulateRequest(new Uri("http://foo?eventId=42"), HttpVerb.GET);
		}

		[Test]
		public void It_should_create_the_calendar()
		{
			CalendarItemRepository.AssertWasCalled(x => x.CreateCalendar(_event));
		}

		[Test]
		public void It_should_render_the_calendar()
		{
			_calendar.AssertWasCalled(x => x.Render(null), o => o.IgnoreArguments());
		}

		[Test]
		public void It_should_set_Http_200_status()
		{
			Assert.AreEqual(200, HttpContext.Current.Response.StatusCode);
		}
	}

	public class When_the_event_calendar_is_requested : With_calendar_handler
	{
		ICalendar _calendar;
		List<Post> _events;

		protected override void Establish_context()
		{
			base.Establish_context();

			_events = new List<Post>
			          {
			          	Create.New.Event()
			          		.Id(42)
			          		.StartingAt(DateTime.MinValue)
			          		.To(DateTime.MinValue.AddDays(10))
			          		.AtLocation("somewhere")
			          		.TheTopicIs("techno babble"),
			          	Create.New.Event()
			          		.Id(43)
			          		.StartingAt(DateTime.MinValue)
			          		.To(DateTime.MinValue.AddDays(10))
			          		.AtLocation("somewhere else")
			          		.TheTopicIs("blah blah")
			          };
			PostRepository.Stub(x => x.GetAll()).Return(_events);

			_calendar = MockRepository.GenerateMock<ICalendar>();
			CalendarItemRepository.Stub(x => x.CreateCalendar(_events))
				.Return(_calendar);
		}

		protected override HttpSimulator CreateRequest()
		{
			return new HttpSimulator().SimulateRequest(new Uri("http://foo"), HttpVerb.GET);
		}

		[Test]
		public void It_should_create_the_calendar()
		{
			CalendarItemRepository.AssertWasCalled(x => x.CreateCalendar(_events));
		}

		[Test]
		public void It_should_render_the_calendar()
		{
			_calendar.AssertWasCalled(x => x.Render(null), o => o.IgnoreArguments());
		}

		[Test]
		public void It_should_set_Http_200_status()
		{
			Assert.AreEqual(200, HttpContext.Current.Response.StatusCode);
		}
	}

	public abstract class With_calendar_handler : Spec
	{
		CalendarHandler _sut;

		protected ICategorizedPostRepository<IEventPluginConfigurationProvider> PostRepository
		{
			get;
			private set;
		}

		HttpSimulator Request
		{
			get;
			set;
		}

		protected ICalendarItemRepository CalendarItemRepository
		{
			get;
			private set;
		}

		protected override void Establish_context()
		{
			PostRepository = MockRepository.GenerateStub<ICategorizedPostRepository<IEventPluginConfigurationProvider>>();
			CalendarItemRepository = MockRepository.GenerateStub<ICalendarItemRepository>();

			_sut = new CalendarHandler(PostRepository, CalendarItemRepository, MockRepository.GenerateMock<ILogger>());

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