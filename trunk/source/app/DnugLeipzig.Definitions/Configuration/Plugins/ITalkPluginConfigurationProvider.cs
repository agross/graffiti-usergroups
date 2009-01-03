namespace DnugLeipzig.Definitions.Configuration.Plugins
{
	public interface ITalkPluginConfigurationProvider : IPluginConfigurationProvider
	{
		string DateField
		{
			get;
		}
	}
}