using System;

namespace DnugLeipzig.Extensions.Configuration
{
	public interface IEventConfigurationSource : IConfigurationSource
	{
		string DateFormat
		{
			get;
		}

		string EndDateField
		{
			get;
		}

		string LocationField
		{
			get;
		}

		string ShortEndDateFormat
		{
			get;
		}

		string StartDateField
		{
			get;
		}

		string UnknownText
		{
			get;
		}
	}
}