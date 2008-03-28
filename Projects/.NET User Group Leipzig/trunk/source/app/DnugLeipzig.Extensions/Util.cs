using System;

using Graffiti.Core;

namespace DnugLeipzig.Extensions
{
	public static class Util
	{
		public static bool IsEvent(Post post)
		{
			if (post == null)
			{
				return false;
			}

			if (!post.Category.Name.Equals("Veranstaltungen", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}

			return true;
		}
	}
}