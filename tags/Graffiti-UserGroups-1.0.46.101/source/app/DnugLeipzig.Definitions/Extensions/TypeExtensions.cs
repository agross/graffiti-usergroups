using System;

namespace DnugLeipzig.Definitions.Extensions
{
	public static class TypeExtensions
	{
		public static string GetPluginName(this Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}

			return String.Format("{0}, {1}", type.FullName, type.Assembly.GetName().Name);
		}
	}
}