namespace DnugLeipzig.Definitions.Configuration
{
	public interface IGraffitiSiteSettings
	{
		string BaseUrl
		{
			get;
		}

		string Title
		{
			get;
		}

		string TagLine
		{
			get;
		}

		string ExternalFeedUrl
		{
			get;
		}

		string Theme
		{
			get;
		}

		string CopyRight
		{
			get;
		}

		double TimeZoneOffSet
		{
			get;
		}

		string Header
		{
			get;
		}

		string WebStatistics
		{
			get;
		}

		int FeaturedId
		{
			get;
		}

		string EmailServer
		{
			get;
		}

		string EmailFrom
		{
			get;
		}

		bool EmailRequiresSSL
		{
			get;
		}

		bool EmailServerRequiresAuthentication
		{
			get;
		}

		int EmailPort
		{
			get;
		}

		string EmailUser
		{
			get;
		}

		string EmailPassword
		{
			get;
		}

		string MetaDescription
		{
			get;
		}

		string MetaKeywords
		{
			get;
		}

		bool UseCustomHomeList
		{
			get;
		}

		bool DisplayGraffitiLogo
		{
			get;
		}
	}
}