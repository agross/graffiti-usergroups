namespace DnugLeipzig.Definitions.Configuration.Plugins
{
	public interface IEventPluginConfigurationProvider : IPluginConfigurationProvider
	{
		string DateFormat
		{
			get;
		}

		string EndDateField
		{
			get;
		}

		string LocationField
		{
			get;
		}

		string ShortEndDateFormat
		{
			get;
		}

		string StartDateField
		{
			get;
		}

		string UnknownText
		{
			get;
		}

		string LocationUnknownField
		{
			get;
		}

		string RegistrationNeededField
		{
			get;
		}

		string RegistrationRecipientField
		{
			get;
		}

		string MaximumNumberOfRegistrationsField
		{
			get;
		}

		string RegistrationListField
		{
			get;
		}

		string RegistrationMailSubject
		{
			get;
		}
	}
}