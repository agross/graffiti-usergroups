using System;
using System.Collections.Specialized;

namespace DnugLeipzig.Extensions.Extensions
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
	}
}