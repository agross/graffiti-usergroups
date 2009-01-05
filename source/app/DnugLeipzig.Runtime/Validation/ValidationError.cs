using DnugLeipzig.Definitions.Validation;

namespace DnugLeipzig.Runtime.Validation
{
	public class ValidationError : INotification
	{
		public ValidationError(string message)
		{
			Message = message;
		}

		#region Implementation of INotification
		public string Message
		{
			get;
			protected set;
		}
		#endregion
	}
}