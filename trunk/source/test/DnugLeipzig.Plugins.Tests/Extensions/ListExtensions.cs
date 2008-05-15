using System;
using System.Collections.Generic;

using DnugLeipzig.Plugins.Migration;

using Graffiti.Core;

namespace DnugLeipzig.Plugins.Tests.Extensions
{
	internal static class ListExtensions
	{
		internal static List<CustomField> ToCustomFieldList(this List<FieldInfo> fields)
		{
			List<CustomField> result = new List<CustomField>();

			fields.ForEach(
				info =>
				result.Add(new CustomField { Description = info.Description, Name = info.FieldName, FieldType = info.FieldType }));

			return result;
		}
	}
}