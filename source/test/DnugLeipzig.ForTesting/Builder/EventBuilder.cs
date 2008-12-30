using System.Web;

using DnugLeipzig.Definitions.Builders;
using DnugLeipzig.Definitions.Configuration.Plugins;

using Graffiti.Core;

namespace DnugLeipzig.ForTesting.Builder
{
	public class EventBuilder : EntityBuilder<Post>
	{
		readonly IEventPluginConfiguration _config;
		string _endDate;
		string _location;
		string _locationUnknown;
		string _maximumNumberOfRegistrations;
		string _registrationRecipient;
		string _startDate;
		string _topic;

		public EventBuilder(IEventPluginConfiguration configuration)
		{
			_config = configuration;
		}

		protected override Post BuildInstance()
		{
			Post result = new Post { Title = HttpUtility.HtmlEncode(_topic) };
			result[_config.StartDateField] = _startDate;
			result[_config.EndDateField] = _endDate;
			result[_config.LocationField] = _location;
			result[_config.LocationUnknownField] = _locationUnknown;
			result[_config.MaximumNumberOfRegistrationsField] = _maximumNumberOfRegistrations;
			result[_config.RegistrationRecipientField] = _registrationRecipient;
			return result;
		}

		public static implicit operator Post(EventBuilder builder)
		{
			return builder.BuildInstance();
		}

		public EventBuilder From(object date)
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

		public EventBuilder WithMaximumNumberOfRegistrations(string value)
		{
			_maximumNumberOfRegistrations = value;
			return this;
		}

		public EventBuilder WithRegistrationRecipient(string email)
		{
			_registrationRecipient = email;
			return this;
		}

		public EventBuilder TheTopicIs(string topic)
		{
			_topic = topic;
			return this;
		}
	}
}