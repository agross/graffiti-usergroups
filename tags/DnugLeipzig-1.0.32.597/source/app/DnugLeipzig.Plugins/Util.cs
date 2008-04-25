using System;
using System.Collections.Generic;

using Graffiti.Core;

namespace DnugLeipzig.Plugins
{
	internal static class Util
	{
		public static bool ValidateExistingCategory(string categoryName)
		{
			var data = new Data();

			return data.GetCategory(categoryName) != null;
		}

		public static bool StringToBoolean(string value)
		{
			return String.Equals(value, "on", StringComparison.InvariantCultureIgnoreCase);
		}

		public static Dictionary<string, string> InitializeFieldNamesFromOldValues(params string[] fieldNames)
		{
			Dictionary<string, string> fields = new Dictionary<string, string>();

			foreach (string fieldName in fieldNames)
			{
				fields.Add(fieldName, null);
			}

			return fields;
		}
	}
}