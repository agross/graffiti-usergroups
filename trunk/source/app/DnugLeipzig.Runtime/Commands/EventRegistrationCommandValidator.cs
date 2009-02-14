using System.Linq;

using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Definitions.Extensions;
using DnugLeipzig.Runtime.Validation;

namespace DnugLeipzig.Runtime.Commands
{
	public class EventRegistrationCommandValidator : Validator<IEventRegistrationCommand>
	{
		public EventRegistrationCommandValidator()
		{
			If(x => x.EventsToRegister == null || !x.EventsToRegister.Count().IsInRange(1, int.MaxValue))
				.AddNotification(EventRegistrationErrors.NoEventSelected);

			IfNot(x => x.Name.HasValue())
				.AddNotification(EventRegistrationErrors.NameIsMissing);

			IfNot(x => x.FormOfAddress.HasValue())
				.AddNotification(EventRegistrationErrors.FormOfAddressIsMissing);

			IfNot(x => x.Occupation.HasValue())
				.AddNotification(EventRegistrationErrors.OccupationIsMissing);

			IfNot(x => x.AttendeeEmail.IsEmail())
				.AddNotification(x => EventRegistrationErrors.EmailIsInvalid(x.AttendeeEmail));
		}
	}
}