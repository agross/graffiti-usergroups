using System;
using System.Runtime.Serialization;

namespace DnugLeipzig.Plugins
{
	[Serializable]
	public class ValidationException : Exception
	{
		public string[] AffectedFields
		{
			get;
			protected set;
		}

		public override string Message
		{
			get
			{
				string affectedFields = String.Empty;

				if (AffectedFields.Length == 1)
				{
					affectedFields = String.Format("Affected field: {0}", AffectedFields);
				}

				if (AffectedFields.Length > 1)
				{
					affectedFields = String.Format("Affected fields: {0}", String.Join(", ", AffectedFields));
				}

				return String.Format("{0} {1}", base.Message, affectedFields).Trim();
			}
		}

		public ValidationException(string message, params string[] affectedFields)
			: this(message, null, affectedFields)
		{
		}

		public ValidationException(string message, Exception inner, params string[] affectedFields)
			: base(message, inner)
		{
			AffectedFields = affectedFields;
		}

		protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			AffectedFields = info.GetValue("AffectedFields", typeof(string[])) as string[];
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("AffectedFields", AffectedFields);
		}
	}
}