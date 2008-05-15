using System;
using System.Collections.Generic;

namespace DnugLeipzig.Plugins.Migration
{
	internal static class MementoHelper
	{
		internal static Dictionary<string, string> GetChangedFieldNames(IMemento oldState, IMemento newState)
		{
			Dictionary<string, string> result = new Dictionary<string, string>();

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

				result.Add(oldField.Value.FieldName, newState.Fields[oldField.Key].FieldName);
			}

			return result;
		}
	}
}