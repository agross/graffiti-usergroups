using Graffiti.Core;

namespace DnugLeipzig.Runtime.Plugins
{
	public class ExceptionTemplate
	{
		public StatusType StatusType
		{
			get;
			set;
		}

		public string Message
		{
			get;
			set;
		}

		public string[] AffectedFormFields
		{
			get;
			set;
		}

		public bool Failed
		{
			get { return StatusType == StatusType.Error || StatusType == StatusType.Warning; }
		}

		public void ThrowAsException()
		{
			throw new ValidationException(Message, null, StatusType, AffectedFormFields);
		}
	}
}