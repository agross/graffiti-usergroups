using System;
using System.Collections.Generic;

using Graffiti.Core;

namespace DnugLeipzig.Plugins.Migration
{
	class MigrationInfo
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

			OldCategoryName = oldState.CategoryName;
			NewCategoryName = newState.CategoryName;

			// Merge fields of the old and new state into one Dictionary.
			ChangedFieldNames = new Dictionary<string, string>();
			foreach (var oldField in oldState.Fields)
			{
				// Skip empty old field values.
				if (String.IsNullOrEmpty(oldField.Value.FieldName))
				{
					continue;
				}

				// Skip field names that did not change.
				if (String.Equals(oldField.Value.FieldName, newState.Fields[oldField.Key].FieldName))
				{
					continue;
				}

				ChangedFieldNames.Add(oldField.Value.FieldName, newState.Fields[oldField.Key].FieldName);
			}

			FieldTypes = new Dictionary<string, FieldType>();
			foreach (var newField in newState.Fields)
			{
				FieldTypes.Add(newField.Value.FieldName, newField.Value.FieldType);
			}
		}

		public string OldCategoryName
		{
			get;
			protected set;
		}

		public string NewCategoryName
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

		/// <summary>
		/// Field types, keyed by the new field name.
		/// </summary>
		public Dictionary<string, FieldType> FieldTypes
		{
			get;
			protected set;
		}
	}
}