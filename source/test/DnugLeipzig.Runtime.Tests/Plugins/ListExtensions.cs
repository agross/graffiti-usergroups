using System.Collections.Generic;

using DnugLeipzig.Runtime.Plugins.Migration;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Tests.Plugins
{
	internal static class ListExtensions
	{
		internal static List<CustomField> ToCustomFieldList(this List<FieldInfo> fields)
		{
			List<CustomField> result = new List<CustomField>();

			fields.ForEach(info => result.Add(new CustomField
			                                  {
			                                  	Description = info.Description,
			                                  	Name = info.FieldName,
			                                  	FieldType = info.FieldType
			                                  }));

			return result;
		}
	}
}