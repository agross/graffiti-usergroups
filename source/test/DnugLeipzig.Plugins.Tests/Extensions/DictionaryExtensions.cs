using System.Collections.Generic;

using Graffiti.Core;

namespace DnugLeipzig.Plugins.Tests.Extensions
{
	static class DictionaryExtensions
	{
		internal static void AddRange(this Dictionary<string, FieldType> dictionary, Dictionary<string, FieldType> other)
		{
			foreach (KeyValuePair<string, FieldType> kvp in other)
			{
				dictionary.Add(kvp.Key, kvp.Value);
			}
		}
	}
}