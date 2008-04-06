using System;
using System.Collections.Generic;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Filters
{
	internal class SortForIndexAscending : ByDate
	{
		public SortForIndexAscending(string dateFieldName)
			: base(dateFieldName)
		{
		}

		#region IPostFilter Members
		public override List<Post> Execute(List<Post> posts)
		{
			if (posts == null || posts.Count <= 1)
			{
				return posts;
			}

			posts.Sort(new DateAscendingPostComparer(_dateFieldName));
			return posts;
		}
		#endregion
	}
}