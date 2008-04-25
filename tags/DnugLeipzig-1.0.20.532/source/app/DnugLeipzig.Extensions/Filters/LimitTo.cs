using System;
using System.Linq;
using System.Collections.Generic;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Filters
{
	internal class LimitTo : IPostFilter
	{
		readonly int _maximumNumberOfPosts;

		public LimitTo(int maximumNumberOfPosts)
		{
			_maximumNumberOfPosts = maximumNumberOfPosts;
		}

		#region IPostFilter Members
		public List<Post> Execute(List<Post> posts)
		{
			return posts.Take(_maximumNumberOfPosts).ToList();
		}
		#endregion
	}
}