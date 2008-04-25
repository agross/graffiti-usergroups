namespace DnugLeipzig.Definitions.Configuration
{
	public interface IPluginConfiguration : ICategoryEnabledRepositoryConfiguration
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