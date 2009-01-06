using System;
using System.Reflection;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Plugins
{
	internal static class PostExtensions
	{
		// HACK: This will very likely break when Graffiti is updated.
		// Still not fixed in Graffiti CMS 1.2. What are these guys doing all day long?
		public static void ForcePropertyUpdate(this Post post)
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