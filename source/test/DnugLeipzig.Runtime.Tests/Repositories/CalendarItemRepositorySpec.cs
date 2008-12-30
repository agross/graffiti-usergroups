using System;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Configuration;
using DnugLeipzig.Definitions.Configuration.Plugins;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.ForTesting;
using DnugLeipzig.Runtime.Repositories;

using Graffiti.Core;

using MbUnit.Framework;

using Rhino.Mocks;

namespace DnugLeipzig.Runtime.Tests.Repositories
{
	public class When_a_calendar_item_for_an_exiting_event_is_created : With_calendar_item_repository
	{
		protected override Post CreatePost(IEventPluginConfiguration configuration)
		{
			return Create.New.Event(configuration)
				.From(DateTime.MinValue)
				.To(DateTime.MinValue.AddDays(10))
				.AtLocation("somewhere")
				.TheTopicIs("techno babble");
		}

		[Test]
		public void It_should_return_a_calendar_item()
		{
			Assert.IsNotNull(_calendarItem);
		}

		[Test]
		public void It_should_initialize_the_start_date_from_the_post()
		{
			Assert.AreEqual(DateTime.MinValue, _calendarItem.StartDate);
		}

		[Test]
		public void It_should_initialize_the_end_date_from_the_post()
		{
			Assert.AreEqual(DateTime.MinValue.AddDays(10), _calendarItem.EndDate);
		}

		[Test]
		public void It_should_initialize_the_location_from_the_post()
		{
			Assert.AreEqual("somewhere", _calendarItem.Location);
		}

		[Test]
		public void It_should_initialize_the_subject_from_the_post()
		{
			Assert.AreEqual("techno babble", _calendarItem.Subject);
		}

		[Test]
		public void It_should_be_able_to_generate_the_ICS_format()
		{
			_calendarItem.ToString();
			Assert.IsTrue(true);
		}
	}

	public class When_a_calendar_item_for_an_event_without_a_start_date_is_created : With_calendar_item_repository
	{
		protected override Post CreatePost(IEventPluginConfiguration configuration)
		{
			return Create.New.Event(configuration)
				.AtLocation("somewhere")
				.TheTopicIs("techno babble");
		}

		[Test]
		public void It_should_not_return_a_calendar_item()
		{
			Assert.IsNull(_calendarItem);
		}
	}

	public class When_a_calendar_item_for_an_event_with_the_end_date_before_the_start_date_is_created
		: With_calendar_item_repository
	{
		protected override Post CreatePost(IEventPluginConfiguration configuration)
		{
			return Create.New.Event(configuration)
				.From(DateTime.MinValue.AddDays(10))
				.To(DateTime.MinValue)
				.AtLocation("somewhere")
				.TheTopicIs("techno babble");
		}

		[Test]
		public void It_should_not_return_a_calendar_item()
		{
			Assert.IsNull(_calendarItem);
		}
	}

	public class When_a_calendar_item_for_an_exiting_event_is_created_and_the_location_is_unknown
		: With_calendar_item_repository
	{
		protected override Post CreatePost(IEventPluginConfiguration configuration)
		{
			return Create.New.Event(configuration)
				.From(DateTime.MinValue)
				.To(DateTime.MinValue.AddDays(10))
				.AtLocation("somewhere")
				.LocationIsUnknown()
				.TheTopicIs("techno babble");
		}

		[Test]
		public void It_should_return_a_calendar_item()
		{
			Assert.IsNotNull(_calendarItem);
		}

		[Test]
		public void It_should_initialize_the_location_from_the_default_settings()
		{
			Assert.AreEqual("to be announced", _calendarItem.Location);
		}
	}

	public abstract class With_calendar_item_repository : Spec
	{
		protected ICalendarItem _calendarItem;
		Post _post;
		CalendarItemRepository _sut;

		protected override void Establish_context()
		{
			var configuration = MockRepository.GenerateMock<IEventPluginConfiguration>();
			var settings = MockRepository.GenerateMock<IGraffitiSiteSettings>();

			_sut = new CalendarItemRepository(configuration,
			                                  settings);

			configuration.Stub(x => x.StartDateField).Return("start");
			configuration.Stub(x => x.EndDateField).Return("end");
			configuration.Stub(x => x.LocationField).Return("location");
			configuration.Stub(x => x.LocationUnknownField).Return("locationunknown");
			configuration.Stub(x => x.UnknownText).Return("to be announced");

			settings.Stub(x => x.BaseUrl).Return("http://foo");

			_post = CreatePost(configuration);
		}

		protected abstract Post CreatePost(IEventPluginConfiguration configuration);

		protected override void Because()
		{
			_calendarItem = _sut.CreateCalendarItemForEvent(_post);
		}
	}
}