using System;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Configuration;
using DnugLeipzig.Definitions.Configuration.Plugins;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.ForTesting;
using DnugLeipzig.Runtime.Repositories;

using MbUnit.Framework;

using Rhino.Mocks;

namespace DnugLeipzig.Runtime.Tests.Repositories
{
	public class When_a_calendar_item_for_an_exiting_event_is_created : With_calendar_item_repository
	{
		protected override void CreatePost(IPostRepository postRepository, IEventPluginConfiguration configuration)
		{
			postRepository.Stub(x => x.GetById(42)).Return(Create.New.Event(configuration)
			                                               	.From(DateTime.MinValue)
			                                               	.To(DateTime.MinValue.AddDays(10))
			                                               	.AtLocation("somewhere")
			                                               	.TheTopicIs("techno babble"));
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

	public class When_a_calendar_item_for_an_exiting_event_is_created_and_the_location_is_unknown
		: With_calendar_item_repository
	{
		protected override void CreatePost(IPostRepository postRepository, IEventPluginConfiguration configuration)
		{
			postRepository.Stub(x => x.GetById(42)).Return(Create.New.Event(configuration)
			                                               	.From(DateTime.MinValue)
			                                               	.To(DateTime.MinValue.AddDays(10))
			                                               	.AtLocation("somewhere")
			                                               	.LocationIsUnknown()
			                                               	.TheTopicIs("techno babble"));
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
		CalendarItemRepository _sut;

		protected override void Establish_context()
		{
			var postRepository = MockRepository.GenerateMock<IPostRepository>();
			var configuration = MockRepository.GenerateMock<IEventPluginConfiguration>();
			var settings = MockRepository.GenerateMock<IGraffitiSiteSettings>();

			_sut = new CalendarItemRepository(postRepository,
			                                  configuration,
			                                  settings);

			configuration.Stub(x => x.StartDateField).Return("start");
			configuration.Stub(x => x.EndDateField).Return("end");
			configuration.Stub(x => x.LocationField).Return("location");
			configuration.Stub(x => x.LocationUnknownField).Return("locationunknown");
			configuration.Stub(x => x.UnknownText).Return("to be announced");

			settings.Stub(x => x.BaseUrl).Return("http://foo");

			CreatePost(postRepository, configuration);
		}

		protected abstract void CreatePost(IPostRepository repository, IEventPluginConfiguration configuration);

		protected override void Because()
		{
			_calendarItem = _sut.GetCalendarItemForEvent(42);
		}
	}
}