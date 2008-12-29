using Graffiti.Core;

namespace DnugLeipzig.Definitions.Configuration
{
	public class GraffitiSiteSettings : SiteSettings
	{
		public GraffitiSiteSettings()
		{
			Get();
		}

		public new string BaseUrl
		{
			get { return SiteSettings.BaseUrl; }
		}
	}
}