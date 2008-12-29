namespace DnugLeipzig.Definitions.Configuration.Plugins
{
	public interface IPluginConfiguration
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