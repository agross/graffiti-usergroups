using System;
using System.Collections.Generic;
using System.Reflection;
using Graffiti.Core;

namespace DnugLeipzig.Runtime.Plugins
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

		// HACK: This will very likely break when Graffiti is updated.
		// Still not fixed in Graffiti CMS 1.2. What are these guys doing all day long?
		public static void ForcePropertyUpdate(Post post)
		{
			if (post == null)
			{
				throw new ArgumentNullException("post");
			}
			
			MethodInfo[] methods = post.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
			MethodInfo method = Array.Find(methods, m => m.Name == "SerializeCustomFields");
			method.Invoke(post, null);
		}
	}
}