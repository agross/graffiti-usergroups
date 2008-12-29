namespace DnugLeipzig.Definitions.Commands.Events
{
	public abstract class EventRegistrationCommand : Command
	{
		public string Name
		{
			get;
			protected set;
		}

		public string FormOfAddress
		{
			get;
			protected set;
		}

		public string Occupation
		{
			get;
			protected set;
		}

		public string AttendeeEmail
		{
			get;
			protected set;
		}

		public bool SendConfirmationToAttendee
		{
			get;
			protected set;
		}
	}
}