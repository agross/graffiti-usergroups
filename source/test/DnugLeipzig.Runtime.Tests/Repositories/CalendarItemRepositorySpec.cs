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
		protected override Post CreatePost()
		{
			return Create.New.Event(ConfigurationProvider)
				.From(DateTime.MinValue)
				.To(DateTime.MinValue.AddDays(10))
				.AtLocation("somewhere")
				.TheTopicIs("techno babble");
		}

		[Test]
		public void It_should_return_a_calendar_item()
		{
			Assert.IsNotNull(CalendarItem);
		}

		[Test]
		public void It_should_initialize_the_start_date_from_the_post()
		{
			Assert.AreEqual(DateTime.MinValue, CalendarItem.StartDate);
		}

		[Test]
		public void It_should_initialize_the_end_date_from_the_post()
		{
			Assert.AreEqual(DateTime.MinValue.AddDays(10), CalendarItem.EndDate);
		}

		[Test]
		public void It_should_initialize_the_location_from_the_post()
		{
			Assert.AreEqual("somewhere", CalendarItem.Location);
		}

		[Test]
		public void It_should_initialize_the_subject_from_the_post()
		{
			Assert.AreEqual("techno babble", CalendarItem.Subject);
		}

		[Test]
		public void It_should_be_able_to_generate_the_ICS_format()
		{
			CalendarItem.ToString();
			Assert.IsTrue(true);
		}
	}

	public class When_a_calendar_item_for_an_event_without_a_start_date_is_created : With_calendar_item_repository
	{
		protected override Post CreatePost()
		{
			return Create.New.Event(ConfigurationProvider)
				.AtLocation("somewhere")
				.TheTopicIs("techno babble");
		}

		[Test]
		public void It_should_not_return_a_calendar_item()
		{
			Assert.IsNull(CalendarItem);
		}
	}

	public class When_a_calendar_item_for_an_event_with_the_end_date_before_the_start_date_is_created
		: With_calendar_item_repository
	{
		protected override Post CreatePost()
		{
			return Create.New.Event(ConfigurationProvider)
				.From(DateTime.MinValue.AddDays(10))
				.To(DateTime.MinValue)
				.AtLocation("somewhere")
				.TheTopicIs("techno babble");
		}

		[Test]
		public void It_should_not_return_a_calendar_item()
		{
			Assert.IsNull(CalendarItem);
		}
	}

	public class When_a_calendar_item_for_an_exiting_event_is_created_and_the_location_is_unknown
		: With_calendar_item_repository
	{
		protected override Post CreatePost()
		{
			return Create.New.Event(ConfigurationProvider)
				.From(DateTime.MinValue)
				.To(DateTime.MinValue.AddDays(10))
				.AtLocation("somewhere")
				.LocationIsUnknown()
				.TheTopicIs("techno babble");
		}

		[Test]
		public void It_should_return_a_calendar_item()
		{
			Assert.IsNotNull(CalendarItem);
		}

		[Test]
		public void It_should_initialize_the_location_from_the_default_settings()
		{
			Assert.AreEqual(ConfigurationProvider.UnknownText, CalendarItem.Location);
		}
	}

	public abstract class With_calendar_item_repository : Spec
	{
		Post _post;
		CalendarItemRepository _sut;

		protected internal ICalendarItem CalendarItem
		{
			get;
			private set;
		}

		protected IEventPluginConfigurationProvider ConfigurationProvider
		{
			get;
			private set;
		}

		protected override void Establish_context()
		{
			ConfigurationProvider = Create.New.StubbedEventPluginConfiguration().Build();
			MockRepository.GenerateMock<IEventPluginConfigurationProvider>();
			var settings = MockRepository.GenerateMock<IGraffitiSiteSettings>();

			_sut = new CalendarItemRepository(ConfigurationProvider,
			                                  settings);

			settings.Stub(x => x.BaseUrl).Return("http://foo");

			_post = CreatePost();
		}

		protected abstract Post CreatePost();

		protected override void Because()
		{
			CalendarItem = _sut.CreateCalendarItemForEvent(_post);
		}
	}
}