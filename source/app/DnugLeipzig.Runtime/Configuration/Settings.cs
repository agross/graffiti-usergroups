using DnugLeipzig.Definitions.Configuration;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Configuration
{
	public class Settings : IGraffitiSettings
	{
		#region IGraffitiSettings Members
		public CommentSettings Comments
		{
			get { return CommentSettings.Get(); }
		}

		public GraffitiSiteSettings Site
		{
			get { return new GraffitiSiteSettings(); }
		}
		#endregion
	}
}