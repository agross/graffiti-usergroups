namespace DnugLeipzig.Definitions.Configuration
{
	public interface ICategoryEnabledRepositoryConfigurationSource
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