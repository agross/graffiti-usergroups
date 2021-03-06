using DnugLeipzig.Definitions.Validation;

namespace DnugLeipzig.Runtime.Validation
{
	public class ValidationError : INotification
	{
		public ValidationError(string message, params string[] affectedFormFields)
		{
			Message = message;
			AffectedFormFields = affectedFormFields;
		}

		#region Implementation of INotification
		public Severity Severity
		{
			get { return Severity.Error; }
		}

		public string Message
		{
			get;
			protected set;
		}

		public string[] AffectedFormFields
		{
			get;
			protected set;
		}
		#endregion
	}
}