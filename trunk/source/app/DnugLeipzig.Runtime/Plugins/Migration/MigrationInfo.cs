using System;
using System.Collections.Generic;

using DnugLeipzig.Runtime.Plugins.Migration;

namespace DnugLeipzig.Runtime.Plugins.Migration
{
	internal class MigrationInfo
	{
		public MigrationInfo(IMemento oldState, IMemento newState)
		{
			if (oldState == null)
			{
				throw new ArgumentNullException("oldState");
			}

			if (newState == null)
			{
				throw new ArgumentNullException("newState");
			}

			SourceCategoryName = oldState.CategoryName;
			TargetCategoryName = newState.CategoryName;

			ChangedFieldNames = MementoHelper.GetChangedFieldNames(oldState, newState);

			AllFields = new List<FieldInfo>();
			foreach (var newField in newState.Fields)
			{
				AllFields.Add(newField.Value);
			}
		}

		public string SourceCategoryName
		{
			get;
			protected set;
		}

		public string TargetCategoryName
		{
			get;
			protected set;
		}

		/// <summary>
		/// Old and new field names.
		/// </summary>
		public Dictionary<string, string> ChangedFieldNames
		{
			get;
			protected set;
		}

		public List<FieldInfo> AllFields
		{
			get;
			protected set;
		}
	}
}