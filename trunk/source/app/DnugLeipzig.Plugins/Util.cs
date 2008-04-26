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

		// HACK: This will very likely break when Graffiti is updated.
		public static void ForcePropertyUpdate(Post post)
		{
			if (post == null)
			{
				throw new ArgumentNullException("post");
			}

			MethodInfo[] methods = post.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
			MethodInfo method = Array.Find(methods,
			                               m =>
			                               m.ReturnType == typeof(void) && m.IsHideBySig && !m.IsFamily &&
			                               m.GetParameters().Length == 0 && m.MetadataToken == 100663972);
			method.Invoke(post, null);
		}
	}
}