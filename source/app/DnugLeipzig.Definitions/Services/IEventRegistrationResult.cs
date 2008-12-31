namespace DnugLeipzig.Definitions.Services
{
	public interface IEventRegistrationResult
	{
		bool OnWaitingList
		{
			get;
		}

		bool AlreadyRegistered
		{
			get;
		}

		bool ErrorOccurred
		{
			get;
		}

		int EventId
		{
			get;
		}
	}
}