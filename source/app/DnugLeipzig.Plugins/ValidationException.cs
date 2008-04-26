using System;
using System.Runtime.Serialization;

using Graffiti.Core;

namespace DnugLeipzig.Plugins
{
	[Serializable]
	public class ValidationException : Exception
	{
		public ValidationException(string message, params string[] affectedFields)
			: this(message, null, StatusType.Error, affectedFields)
		{
		}

		public ValidationException(string message, StatusType severity)
			: this(message, null, severity, null)
		{
		}

		public ValidationException(string message, Exception inner, params string[] affectedFields)
			: this(message, inner, StatusType.Error, affectedFields)
		{
		}

		public ValidationException(string message, Exception inner, StatusType severity, params string[] affectedFields)
			: base(message, inner)
		{
			AffectedFields = affectedFields;
			Severity = severity;
		}

		protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			AffectedFields = info.GetValue("AffectedFields", typeof(string[])) as string[];
			Severity = (StatusType) info.GetValue("Severity", typeof(StatusType));
		}

		public string[] AffectedFields
		{
			get;
			protected set;
		}

		public StatusType Severity
		{
			get;
			protected set;
		}

		public override string Message
		{
			get
			{
				string affectedFields = String.Empty;

				if (AffectedFields != null)
				{
					if (AffectedFields.Length == 1)
					{
						affectedFields = String.Format("Affected field: {0}", AffectedFields);
					}

					if (AffectedFields.Length > 1)
					{
						affectedFields = String.Format("Affected fields: {0}", String.Join(", ", AffectedFields));
					}
				}

				return String.Format("{0} {1}", base.Message, affectedFields).Trim();
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("AffectedFields", AffectedFields);
			info.AddValue("Severity", Severity);
		}
	}
}