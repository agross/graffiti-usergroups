using System.Diagnostics;

using Graffiti.Core;

namespace DnugLeipzig.Plugins.Migration
{
	[DebuggerDisplay("FieldName = {FieldName}, FieldType = {FieldType}")]
	internal class FieldInfo
	{
		public FieldInfo(string fieldName, FieldType fieldType)
		{
			FieldName = fieldName;
			FieldType = fieldType;
		}

		public string FieldName
		{
			get;
			protected set;
		}

		public FieldType FieldType
		{
			get;
			protected set;
		}
	}
}