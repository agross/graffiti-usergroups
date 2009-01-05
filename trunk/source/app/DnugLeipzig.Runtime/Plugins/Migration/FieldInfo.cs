using System;
using System.Diagnostics;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Plugins.Migration
{
	[DebuggerDisplay("FieldName = {FieldName}, FieldType = {FieldType}, Description = {Description}")]
	public class FieldInfo:IEquatable<FieldInfo>
	{
		public FieldInfo(string fieldName, FieldType fieldType, string description)
		{
			FieldName = fieldName;
			FieldType = fieldType;
			Description = description;
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

		public string Description
		{
			get;
			protected set;
		}

		#region IEquatable<FieldInfo> Members
		public bool Equals(FieldInfo other)
		{
			return String.Equals(FieldName, other.FieldName, StringComparison.OrdinalIgnoreCase);
		}
		#endregion
	}
}