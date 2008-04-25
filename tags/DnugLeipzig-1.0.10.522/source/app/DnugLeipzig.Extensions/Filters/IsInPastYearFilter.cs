using System;
using System.Collections.Generic;
using System.Linq;

using DnugLeipzig.Extensions.Extensions;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Filters
{
	internal class IsInPastYearFilter : ByDateFilter
	{
		public IsInPastYearFilter(string dateFieldName) : base(dateFieldName)
		{
		}

		#region IPostFilter Members
		public override List<Post> Execute(List<Post> posts)
		{
			return (from post in posts
			       where
			       	post.Custom(_dateFieldName).AsEventDate().Year < DateTime.Now.Year
			       select post).ToList();
		}
		#endregion
	}
}