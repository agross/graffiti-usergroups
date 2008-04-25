using System;

namespace DnugLeipzig.Extensions.Configuration
{
	public interface IRepositoryConfigurationSource
	{
		string CategoryName
		{
			get;
		}

		string SortRelevantDateField
		{
			get;
		}
	}
}