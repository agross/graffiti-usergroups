using System;

namespace DnugLeipzig.Extensions.Configuration
{
	public interface ITalkConfigurationSource : IConfigurationSource
	{
		string DateField
		{
			get;
		}
	}
}