using System;
using System.Web;

using Castle.Core.Logging;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Configuration.Plugins;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.ForTesting;
using DnugLeipzig.ForTesting.HttpMocks;
using DnugLeipzig.Runtime.Handlers;

using Graffiti.Core;

using MbUnit.Framework;

using Rhino.Mocks;
using Rhino.Mocks.Constraints;

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

	public class When_the_calendar_handler_creates_an_item_for_a_non_numeric_event_ID : With_calendar_handler
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

	public class When_the_calendar_handler_creates_an_item_for_a_non_existing_event : With_calendar_handler
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

	public class When_the_calendar_handler_creates_an_item : With_calendar_handler
	{
		ICalendarItem _calendarItem;

		protected override void Establish_context()
		{
			base.Establish_context();

			PostRepository.Stub(x => x.GetById(42)).Return(Create.New.Event(ConfigurationProvider)
			                                               	.Id(42)
			                                               	.From(DateTime.MinValue)
			                                               	.To(DateTime.MinValue.AddDays(10))
			                                               	.AtLocation("somewhere")
			                                               	.TheTopicIs("techno babble"));

			_calendarItem = MockRepository.GenerateMock<ICalendarItem>();
			CalendarItemRepository.Stub(x => x.CreateCalendarItemForEvent(null))
				.IgnoreArguments()
				.Return(_calendarItem);
		}

		protected override HttpSimulator CreateRequest()
		{
			return new HttpSimulator().SimulateRequest(new Uri("http://foo?eventId=42"), HttpVerb.GET);
		}

		[Test]
		public void It_should_return_the_calendar_item_from_the_repository()
		{
			CalendarItemRepository.AssertWasCalled(x => x.CreateCalendarItemForEvent(null),
			                                       o => o.Constraints(Is.Matching<Post>(p => p.Id == 42)));
		}

		[Test]
		public void It_should_render_the_returned_calendar_item()
		{
			_calendarItem.AssertWasCalled(x => x.Render(null), o => o.IgnoreArguments());
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

		protected IEventPluginConfigurationProvider ConfigurationProvider
		{
			get;
			private set;
		}

		protected IPostRepository PostRepository
		{
			get;
			private set;
		}

		protected HttpSimulator Request
		{
			get;
			private set;
		}

		protected ICalendarItemRepository CalendarItemRepository
		{
			get;
			private set;
		}

		protected override void Establish_context()
		{
			PostRepository = MockRepository.GenerateStub<IPostRepository>();
			CalendarItemRepository = MockRepository.GenerateStub<ICalendarItemRepository>();
			ConfigurationProvider = Create.New.StubbedEventPluginConfiguration().Build();

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