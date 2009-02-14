namespace DnugLeipzig.Definitions.Plugins
{
	public interface IPluginConfigurationProvider
	{
		string CategoryName
		{
			get;
		}

		string SortRelevantDateField
		{
			get;
		}

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