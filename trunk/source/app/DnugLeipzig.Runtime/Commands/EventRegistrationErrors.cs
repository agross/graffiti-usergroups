using System;

using DnugLeipzig.Definitions.Extensions;
using DnugLeipzig.Definitions.Validation;
using DnugLeipzig.Runtime.Validation;

namespace DnugLeipzig.Runtime.Commands
{
	public static class EventRegistrationErrors
	{
		// TODO: English
		public static readonly INotification FormOfAddressIsMissing = new ValidationError("Please select a form of address.");
		public static readonly INotification NameIsMissing = new ValidationError("Please enter your name.");

		public static readonly INotification NoEventSelected =
			new ValidationError("Please select at least one event to register for.");

		public static readonly INotification OccupationIsMissing = new ValidationError("Please select your occupation.");

		public static INotification EmailIsInvalid(string emailAddress)
		{
			if (emailAddress.IsNullOrEmpty())
			{
				return new ValidationError("Please enter your e-mail address.");
			}

			return new ValidationError(String.Format("The e-mail address '{0}' is invalid.", emailAddress));
		}
	}
}