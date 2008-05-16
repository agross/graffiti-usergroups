using System.Collections.Generic;

namespace DnugLeipzig.Runtime
{
	public class RegistrationRequest
	{
		public ICollection<int> RegisteredEvents
		{
			get;
			set;
		}

		public string FormOfAddress
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string Occupation
		{
			get;
			set;
		}

		public string AttendeeEMail
		{
			get;
			set;
		}

		public bool CcToAttendee
		{
			get;
			set;
		}

		public RegistrationRequest()
		{
			RegisteredEvents = new List<int>();
		}

		public ICollection<string> Validate()
		{
			// TODO: English.
			List<string> validationErrors = new List<string>();
			if (!Validator.ValidateRange(RegisteredEvents.Count, 1, int.MaxValue))
			{
				validationErrors.Add("Please select at least one event to register for.");
			}

			if(!Validator.ValidateExisting(FormOfAddress))
			{
				validationErrors.Add("Please select a form of address.");
			}

			if (!Validator.ValidateExisting(Name))
			{
				validationErrors.Add("Please enter your name.");
			}
			
			if (!Validator.ValidateExisting(Occupation))
			{
				validationErrors.Add("Please select your occupation.");
			}

			if (!Validator.ValidateEmail(AttendeeEMail))
			{
				validationErrors.Add("Please enter your e-mail address.");
			}

			return validationErrors;
		}
	}
}