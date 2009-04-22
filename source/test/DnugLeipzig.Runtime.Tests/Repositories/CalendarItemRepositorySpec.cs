using System;
using System.Collections.Generic;
using System.Linq;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.GraffitiIntegration;
using DnugLeipzig.Definitions.Plugins.Events;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.ForTesting;
using DnugLeipzig.Runtime.Repositories;

using Graffiti.Core;

using MbUnit.Framework;

using Rhino.Mocks;

namespace DnugLeipzig.Runtime.Tests.Repositories
{
	public class When_a_calendar_item_for_an_exiting_event_is_created : With_single_event
	{
		protected override Post CreatePost()
		{
			return Create.New.Event()
				.StartingAt(DateTime.MinValue)
				.To(DateTime.MinValue.AddDays(10))
				.AtLocation("somewhere")
				.TheTopicIs("techno babble");
		}

		[Test]
		public void It_should_return_a_calendar()
		{
			Assert.IsNotNull(Calendar);
		}
		
		[Test]
		public void It_should_return_a_calendar_with_one_entry()
		{
			Assert.AreEqual(1, Calendar.Items.Count);
		}

		[Test]
		public void It_should_initialize_the_start_date_from_the_post()
		{
			Assert.AreEqual(DateTime.MinValue, Calendar.Items.First().StartDate);
		}

		[Test]
		public void It_should_initialize_the_end_date_from_the_post()
		{
			Assert.AreEqual(DateTime.MinValue.AddDays(10), Calendar.Items.First().EndDate);
		}

		[Test]
		public void It_should_initialize_the_location_from_the_post()
		{
			Assert.AreEqual("somewhere", Calendar.Items.First().Location);
		}

		[Test]
		public void It_should_initialize_the_subject_from_the_post()
		{
			Assert.AreEqual("techno babble", Calendar.Items.First().Subject);
		}

		[Test]
		public void It_should_be_able_to_generate_the_ICS_format()
		{
			StringAssert.Contains(Calendar.ToString(), "techno babble");
		}

		[Test]
		public void It_should_not_generate_the_ICS_header()
		{
			StringAssert.NotLike(Calendar.ToString(), "%BEGIN:VCALENDAR%");
		}
	}

	public class When_the_calendar_is_created : With_calendar_item_repository
	{
		ICalendar _calendar;

		protected override void Because()
		{
			_calendar = _sut.CreateCalendar(new List<Post>
			                                {
			                                	Create.New.Event()
			                                		.StartingAt(DateTime.MinValue)
			                                		.To(DateTime.MinValue.AddDays(10))
			                                		.AtLocation("somewhere")
			                                		.TheTopicIs("techno babble"),
			                                	Create.New.Event()
			                                		.StartingAt(DateTime.MinValue)
			                                		.To(DateTime.MinValue.AddDays(10))
			                                		.AtLocation("somewhere")
			                                		.TheTopicIs("techno babble")
			                                });
		}

		[Test]
		public void It_should_create_items_for_each_event()
		{
			Assert.AreEqual(2, _calendar.Items.Count);
		}
		
		[Test]
		public void It_should_be_able_to_generate_the_ICS_format()
		{
			StringAssert.Contains(_calendar.ToString(), "techno babble");
		}
		
		[Test]
		public void It_should_generate_a_single_ICS_header()
		{
			Assert.AreEqual(_calendar.ToString().IndexOf("BEGIN:VCALENDAR"), _calendar.ToString().LastIndexOf("BEGIN:VCALENDAR"));
		}

		[Test]
		public void It_should_set_the_calendar_name_to_the_site_name()
		{
			StringAssert.Contains(_calendar.ToString(), _settings.Title);
		}
	}

	public class When_a_calendar_item_for_an_event_without_a_start_date_is_created : With_single_event
	{
		protected override Post CreatePost()
		{
			return Create.New.Event()
				.AtLocation("somewhere")
				.TheTopicIs("techno babble");
		}

		[Test]
		public void It_should_return_a_calendar()
		{
			Assert.IsNotNull(Calendar);
		}

		[Test]
		public void It_should_return_a_calendar_with_no_entries()
		{
			Assert.AreEqual(0, Calendar.Items.Count);
		}
	}

	public class When_a_calendar_item_for_an_event_with_the_end_date_before_the_start_date_is_created
		: With_single_event
	{
		protected override Post CreatePost()
		{
			return Create.New.Event()
				.StartingAt(DateTime.MinValue.AddDays(10))
				.To(DateTime.MinValue)
				.AtLocation("somewhere")
				.TheTopicIs("techno babble");
		}

		[Test]
		public void It_should_return_a_calendar()
		{
			Assert.IsNotNull(Calendar);
		}

		[Test]
		public void It_should_return_a_calendar_with_no_entries()
		{
			Assert.AreEqual(0, Calendar.Items.Count);
		}
	}

	public class When_a_calendar_item_for_an_exiting_event_is_created_and_the_location_is_unknown
		: With_single_event
	{
		protected override Post CreatePost()
		{
			return Create.New.Event()
				.StartingAt(DateTime.MinValue)
				.To(DateTime.MinValue.AddDays(10))
				.AtLocation("somewhere")
				.LocationIsUnknown()
				.TheTopicIs("techno babble");
		}

		[Test]
		public void It_should_return_a_calendar()
		{
			Assert.IsNotNull(Calendar);
		}

		[Test]
		public void It_should_return_a_calendar_with_one_entry()
		{
			Assert.AreEqual(1, Calendar.Items.Count);
		}

		[Test]
		public void It_should_initialize_the_location_from_the_default_settings()
		{
			Assert.AreEqual(ConfigurationProvider.UnknownText, Calendar.Items.First().Location);
		}
	}

	public abstract class With_calendar_item_repository : Spec
	{
		protected CalendarItemRepository _sut;
		protected IGraffitiSiteSettings _settings;

		protected IEventPluginConfigurationProvider ConfigurationProvider
		{
			get;
			private set;
		}

		protected override void Establish_context()
		{
			ConfigurationProvider = Create.New.StubbedEventPluginConfiguration().Build();
			MockRepository.GenerateMock<IEventPluginConfigurationProvider>();
			_settings = MockRepository.GenerateMock<IGraffitiSiteSettings>();
			_settings.Stub(x => x.Title).Return("The site's title");

			_sut = new CalendarItemRepository(ConfigurationProvider,
			                                  _settings);

			_settings.Stub(x => x.BaseUrl).Return("http://foo");
		}
	}

	public abstract class With_single_event : With_calendar_item_repository
	{
		Post _post;

		protected internal ICalendar Calendar
		{
			get;
			private set;
		}

		protected override void Establish_context()
		{
			base.Establish_context();
			_post = CreatePost();
		}

		protected abstract Post CreatePost();

		protected override void Because()
		{
			Calendar = _sut.CreateCalendarForEvent(_post);
		}
	}
}