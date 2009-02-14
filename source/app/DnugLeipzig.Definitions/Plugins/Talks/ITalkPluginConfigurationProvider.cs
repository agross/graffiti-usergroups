namespace DnugLeipzig.Definitions.Plugins.Talks
{
	public interface ITalkPluginConfigurationProvider : IPluginConfigurationProvider
	{
		string DateField
		{
			get;
		}
	}
}