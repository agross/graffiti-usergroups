using System;
using System.Collections.Specialized;

namespace DnugLeipzig.Definitions.Extensions
{
	public static class NameValueCollectionExtensions
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

		public static void TrimAllValues(this NameValueCollection nvc)
		{
			foreach (string key in nvc.AllKeys)
			{
				if (nvc[key] == null)
				{
					continue;
				}

				nvc[key] = nvc[key].Trim();
			}
		}
	}
}