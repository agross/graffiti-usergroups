namespace DnugLeipzig.Definitions.Configuration
{
	public interface ICategoryEnabledRepositoryConfiguration
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