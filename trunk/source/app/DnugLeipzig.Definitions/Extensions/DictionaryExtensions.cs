using System;
using System.Collections;
using System.Collections.Specialized;

namespace DnugLeipzig.Definitions.Extensions
{
	public static class DictionaryExtensions
	{
		public static string GetOrDefault(this NameValueCollection nvc, string key, string defaultValue)
		{
			if (String.IsNullOrEmpty(key))
			{
				throw new ArgumentOutOfRangeException("key");
			}

			if (nvc.Get(key) == null)
			{
				return defaultValue;
			}

			return nvc.Get(key);
		}

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