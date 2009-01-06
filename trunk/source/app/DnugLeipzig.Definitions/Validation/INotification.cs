namespace DnugLeipzig.Definitions.Validation
{
	public interface INotification
	{
		Severity Severity
		{
			get;
		}

		string Message
		{
			get;
		}

		string[] AffectedFormFields
		{
			get;
		}
	}
}