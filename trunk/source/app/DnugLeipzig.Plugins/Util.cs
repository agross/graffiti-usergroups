using System;
using System.Collections.Generic;
using System.Reflection;

using Graffiti.Core;

namespace DnugLeipzig.Plugins
{
	internal static class Util
	{
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