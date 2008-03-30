using System;
using System.Collections.Generic;
using System.Configuration;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Extensions
{
	public static class ListExtensions
	{
		static readonly string DateFieldName;

		static ListExtensions()
		{
			DateFieldName = ConfigurationManager.AppSettings.GetOrDefault("UserGroup:Events:DateFieldName", "Datum");
		}

		public static void SortForIndexView(this List<Post> posts)
		{
			if (posts == null || posts.Count <= 1)
			{
				return;
			}

			posts.Sort(new PostComparer(DateFieldName));
		}
	}
}