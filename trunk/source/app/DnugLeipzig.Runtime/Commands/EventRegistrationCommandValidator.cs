using System.Linq;

using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Runtime.Validation;

namespace DnugLeipzig.Runtime.Commands
{
	public class EventRegistrationCommandValidator : Validator<IEventRegistrationCommand>
	{
		public EventRegistrationCommandValidator()
		{
			If(x => x.EventsToRegister == null || !x.EventsToRegister.Count().IsInRange(1, int.MaxValue))
				.AddNotification(EventRegistrationErrors.NoEventSelected);

			IfNot(x => x.Name.Exists())
				.AddNotification(EventRegistrationErrors.NameIsMissing);

			IfNot(x => x.FormOfAddress.Exists())
				.AddNotification(EventRegistrationErrors.FormOfAddressIsMissing);

			IfNot(x => x.Occupation.Exists())
				.AddNotification(EventRegistrationErrors.OccupationIsMissing);

			IfNot(x => x.AttendeeEmail.IsEmail())
				.AddNotification(x => EventRegistrationErrors.EmailIsInvalid(x.AttendeeEmail));
		}
	}
}