using System;
using System.Collections.Generic;
using System.Linq;

using DnugLeipzig.Extensions.Extensions;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Filters
{
	internal class IsInYearFilter : ByDateFilter
	{
		DateTime _year;

		public IsInYearFilter(string dateFieldName, DateTime year) : base(dateFieldName)
		{
			_year = year;
		}

		#region IPostFilter Members
		public override List<Post> Execute(List<Post> posts)
		{
			return (from post in posts
			       where post.Custom(_dateFieldName).AsEventDate().Year == _year.Year
			       select post).ToList();
		}
		#endregion
	}
}