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

		string Theme
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

		bool EmailRequiresSsl
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

		bool UseCustomHomeList
		{
			get;
		}

		string DateFormat
		{
			get;
		}
	}
}