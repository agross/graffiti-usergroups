using System;
using System.Collections.Generic;
using System.Linq;

using DnugLeipzig.Extensions.Extensions;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Filters
{
	internal class IsInFuture : ByDate
	{
		public IsInFuture(string dateFieldName)
			: base(dateFieldName)
		{
		}

		#region IPostFilter Members
		public override List<Post> Execute(List<Post> posts)
		{
			return (from post in posts
			       where post.Custom(_dateFieldName).AsEventDate().Date >= DateTime.Now.Date
			       select post).ToList();
		}
		#endregion
	}
}