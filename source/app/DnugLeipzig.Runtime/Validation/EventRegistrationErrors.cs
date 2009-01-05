using System;

namespace DnugLeipzig.Runtime.Validation
{
	public static class EventRegistrationErrors
	{
		// TODO: English
		public static readonly ValidationError FormOfAddressIsMissing = new ValidationError("Please select a form of address.");
		public static readonly ValidationError NameIsMissing = new ValidationError("Please enter your name.");
		public static readonly ValidationError NoEventSelected = new ValidationError("Please select at least one event to register for.");
		public static readonly ValidationError OccupationIsMissing = new ValidationError("Please select your occupation.");

		public static ValidationError EmailIsInvalid<T>(string emailAddress)
		{
			return new ValidationError(String.Format("The e-mail address '{0}' is invalid.", emailAddress));
		}
	}
}