using System.Collections.Generic;

namespace DnugLeipzig.Runtime
{
	public class RegistrationResponse
	{
		public IEnumerable<string> ValidationErrors
		{
			get;
			set;
		}

		public ICollection<int> WaitingListEvents
		{
			get;
			set;
		}

		public bool Success
		{
			get;
			set;
		}

		public RegistrationResponse()
		{
			Success = false;
			ValidationErrors = new List<string>();
			WaitingListEvents = new List<int>();
		}
	}
}