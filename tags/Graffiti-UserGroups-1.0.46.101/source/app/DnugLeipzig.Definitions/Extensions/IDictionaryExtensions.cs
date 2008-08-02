using System;
using System.Collections;

namespace DnugLeipzig.Definitions.Extensions
{
	public static class IDictionaryExtensions
	{
		public static string GetAsAttribute(this IDictionary properties, string key)
		{
			string value = properties[key] as string;
			if (!String.IsNullOrEmpty(value))
			{
				return String.Format("{0}=\"{1}\"", key, value.Trim());
			}

			return null;
		}
	}
}