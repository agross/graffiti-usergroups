using System.Collections.Generic;

using Graffiti.Core;

namespace DnugLeipzig.Plugins.Tests.Extensions
{
	static class ListExtensions
	{
		internal static Dictionary<string, FieldType> ToCustomFieldDictionary(this List<CustomField> fields)
		{
			Dictionary<string, FieldType> result = new Dictionary<string, FieldType>();

			foreach (CustomField field in fields)
			{
				result.Add(field.Name, field.FieldType);
			}

			return result;
		}
	}
}