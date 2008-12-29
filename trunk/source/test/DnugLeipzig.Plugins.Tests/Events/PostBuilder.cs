using Graffiti.Core;

namespace DnugLeipzig.Plugins.Tests.Events
{
	internal class PostBuilder
	{
		readonly EventPlugin _eventPlugin;
		string _endDate;
		string _location;
		string _locationUnknown;
		string _maximumNumberOfRegistrations;
		string _registrationRecipient;
		string _startDate;

		public PostBuilder(EventPlugin eventPlugin)
		{
			_eventPlugin = eventPlugin;
		}

		Post BuildInstance()
		{
			Post result = new Post();
			result[_eventPlugin.StartDateField] = _startDate;
			result[_eventPlugin.EndDateField] = _endDate;
			result[_eventPlugin.LocationField] = _location;
			result[_eventPlugin.LocationUnknownField] = _locationUnknown;
			result[_eventPlugin.MaximumNumberOfRegistrationsField] = _maximumNumberOfRegistrations;
			result[_eventPlugin.RegistrationRecipientField] = _registrationRecipient;
			return result;
		}

		public static implicit operator Post(PostBuilder builder)
		{
			return builder.BuildInstance();
		}

		public PostBuilder WithStartDate(object date)
		{
			_startDate = date == null ? null : date.ToString();
			return this;
		}

		public PostBuilder WithEndDate(object date)
		{
			_endDate = date == null ? null : date.ToString();
			return this;
		}

		public PostBuilder WithLocation(string location)
		{
			_location = location;
			return this;
		}

		public PostBuilder LocationIsUnknown()
		{
			_locationUnknown = "on";
			return this;
		}

		public PostBuilder LocationIsKnown()
		{
			_locationUnknown = "off";
			return this;
		}

		public PostBuilder WithMaximumNumberOfRegistrations(string value)
		{
			_maximumNumberOfRegistrations = value;
			return this;
		}

		public PostBuilder WithRegistrationRecipient(string email)
		{
			_registrationRecipient = email;
			return this;
		}
	}
}