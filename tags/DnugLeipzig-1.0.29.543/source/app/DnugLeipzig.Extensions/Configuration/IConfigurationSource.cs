using System;

namespace DnugLeipzig.Extensions.Configuration
{
	public interface IConfigurationSource : IRepositoryConfigurationSource
	{
		string SpeakerField
		{
			get;
		}

		string YearQueryString
		{
			get;
		}
	}
}