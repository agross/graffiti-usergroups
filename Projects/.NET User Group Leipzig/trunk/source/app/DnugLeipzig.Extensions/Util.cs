using System;
using System.Web;

using DnugLeipzig.Extensions.Extensions;

namespace DnugLeipzig.Extensions
{
	public static class Util
	{
		internal static int? GetYearOfView(string yearQueryStringParameter)
		{
			int year;
			if (int.TryParse(HttpContext.Current.Request.QueryString[yearQueryStringParameter], out year))
			{
				return year;
			}

			return null;
		}

		internal static string GetUrlForYearView(int year, string yearQueryStringParameter)
		{
			bool needsAmpersand = !String.IsNullOrEmpty(HttpContext.Current.Request.Url.Query);

			return String.Format("{0}{1}{2}={3}",
			                     HttpContext.Current.Request.Url.PathAndQuery.StripDefaultAspx(),
			                     needsAmpersand ? "&" : "?",
			                     yearQueryStringParameter,
			                     year);
		}
	}
}