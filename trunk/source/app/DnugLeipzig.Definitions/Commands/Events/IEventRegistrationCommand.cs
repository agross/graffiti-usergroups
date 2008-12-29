namespace DnugLeipzig.Definitions.Commands.Events
{
	public interface IEventRegistrationCommand : ICommand
	{
		string Name
		{
			get;
		}

		string FormOfAddress
		{
			get;
		}

		string Occupation
		{
			get;
		}

		string AttendeeEmail
		{
			get;
		}

		bool SendConfirmationToAttendee
		{
			get;
		}
	}
}