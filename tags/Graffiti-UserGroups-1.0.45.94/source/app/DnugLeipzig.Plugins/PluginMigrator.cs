using DnugLeipzig.Plugins.Migration;

namespace DnugLeipzig.Plugins
{
	public static class PluginMigrator
	{
		public static void MigrateSettings(bool createTargetCategoryAndFields,
		                           bool migrateFieldValues,
		                           IMemento newState,
		                           IMemento oldState)
		{
			Migrator migrator = new Migrator();

			if (createTargetCategoryAndFields)
			{
				migrator.EnsureTargetCategory(newState.CategoryName);
				migrator.EnsureFields(newState.CategoryName, new MigrationInfo(oldState, newState).AllFields);
			}

			if (migrateFieldValues)
			{
				migrator.Migrate(new MigrationInfo(oldState, newState));
			}
		}
	}
}