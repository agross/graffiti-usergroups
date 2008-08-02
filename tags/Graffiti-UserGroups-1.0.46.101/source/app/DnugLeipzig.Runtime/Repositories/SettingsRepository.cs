using DnugLeipzig.Definitions.Repositories;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Repositories
{
	public class SettingsRepository : ISettingsRepository
	{
		#region ISettingsRepository Members
		public CommentSettings CommentSettings
		{
			get { return CommentSettings.Get(); }
		}
		#endregion
	}
}