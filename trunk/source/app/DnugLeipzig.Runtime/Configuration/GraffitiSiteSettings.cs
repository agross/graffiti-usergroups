using DnugLeipzig.Definitions.Configuration;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Configuration
{
	public class GraffitiSiteSettings : SiteSettings, IGraffitiSiteSettings
	{
		public new string BaseUrl
		{
			get { return SiteSettings.BaseUrl; }
		}
	}
}