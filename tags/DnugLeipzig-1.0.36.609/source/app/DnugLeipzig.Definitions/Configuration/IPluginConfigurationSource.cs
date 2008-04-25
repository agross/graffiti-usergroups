namespace DnugLeipzig.Definitions.Configuration
{
	public interface IPluginConfigurationSource : ICategoryEnabledRepositoryConfigurationSource
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