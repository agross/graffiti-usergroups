using System;
using System.Web.Script.Serialization;

namespace DnugLeipzig.Definitions.Extensions
{
	public static class ObjectExtensions
	{
		public static string ToJson(this object instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}

			JavaScriptSerializer serializer = new JavaScriptSerializer();
			return serializer.Serialize(instance);
		}
	}
}