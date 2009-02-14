using DnugLeipzig.Definitions.GraffitiIntegration;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.GraffitiIntegration
{
	public class GraffitiSiteSettings : IGraffitiSiteSettings
	{
		#region IGraffitiSiteSettings Members
		public string BaseUrl
		{
			get { return SiteSettings.BaseUrl; }
		}

		public string Title
		{
			get { return SiteSettings.Get().Title; }
		}

		public string Theme
		{
			get { return SiteSettings.Get().Theme; }
		}

		public string EmailServer
		{
			get { return SiteSettings.Get().EmailServer; }
		}

		public string EmailFrom
		{
			get { return SiteSettings.Get().EmailFrom; }
		}

		public bool EmailRequiresSsl
		{
			get { return SiteSettings.Get().EmailRequiresSSL; }
		}

		public bool EmailServerRequiresAuthentication
		{
			get { return SiteSettings.Get().EmailServerRequiresAuthentication; }
		}

		public int EmailPort
		{
			get { return SiteSettings.Get().EmailPort; }
		}

		public string EmailUser
		{
			get { return SiteSettings.Get().EmailUser; }
		}

		public string EmailPassword
		{
			get { return SiteSettings.Get().EmailPassword; }
		}

		public bool UseCustomHomeList
		{
			get { return SiteSettings.Get().UseCustomHomeList; }
		}

		public string DateFormat
		{
			get { return SiteSettings.DateFormat; }
		}
		#endregion
	}
}