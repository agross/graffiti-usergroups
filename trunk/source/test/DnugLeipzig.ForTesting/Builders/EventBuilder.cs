using System.Web;

using DnugLeipzig.Definitions.Builders;
using DnugLeipzig.Definitions.Plugins.Events;

using Graffiti.Core;

namespace DnugLeipzig.ForTesting.Builders
{
	public class EventBuilder : EntityBuilder<Post>
	{
		readonly IEventPluginConfigurationProvider _config;
		string _endDate;
		int _id;
		string _location;
		string _locationUnknown;
		string _maximumNumberOfRegistrations;
		string _organizerEmail;
		string _registrationList;
		string _startDate;
		string _topic;

		public EventBuilder(IEventPluginConfigurationProvider configurationProvider)
		{
			_config = configurationProvider;
		}

		protected override Post BuildInstance()
		{
			Post result = new Post
			              {
			              	Id = _id,
			              	Title = HttpUtility.HtmlEncode(_topic)
			              };
			result[_config.StartDateField] = _startDate;
			result[_config.EndDateField] = _endDate;
			result[_config.LocationField] = _location;
			result[_config.LocationUnknownField] = _locationUnknown;
			result[_config.MaximumNumberOfRegistrationsField] = _maximumNumberOfRegistrations;
			result[_config.RegistrationRecipientField] = _organizerEmail;
			result[_config.RegistrationListField] = _registrationList;
			return result;
		}

		public EventBuilder Id(int id)
		{
			_id = id;
			return this;
		}

		public EventBuilder StartingAt(object date)
		{
			_startDate = date == null ? null : date.ToString();
			return this;
		}

		public EventBuilder To(object date)
		{
			_endDate = date == null ? null : date.ToString();
			return this;
		}

		public EventBuilder AtLocation(string location)
		{
			_location = location;
			return this;
		}

		public EventBuilder LocationIsUnknown()
		{
			_locationUnknown = "on";
			return this;
		}

		public EventBuilder LocationIsKnown()
		{
			_locationUnknown = "off";
			return this;
		}

		public EventBuilder WillBeBookedUpAfterRegistrationNumber(string value)
		{
			_maximumNumberOfRegistrations = value;
			return this;
		}

		public EventBuilder OrganizedBy(string email)
		{
			_organizerEmail = email;
			return this;
		}

		public EventBuilder TheTopicIs(string topic)
		{
			_topic = topic;
			return this;
		}

		public EventBuilder WithAttendeeList(string registrationList)
		{
			_registrationList = registrationList;
			return this;
		}
	}
}