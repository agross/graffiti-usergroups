using DnugLeipzig.Definitions.Validation;

namespace DnugLeipzig.Runtime.Validation
{
	public class ValidationWarning : INotification
	{
		public ValidationWarning(string message, params string[] affectedFormFields)
		{
			Message = message;
			AffectedFormFields = affectedFormFields;
		}

		#region Implementation of INotification
		public Severity Severity
		{
			get { return Severity.Warning; }
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