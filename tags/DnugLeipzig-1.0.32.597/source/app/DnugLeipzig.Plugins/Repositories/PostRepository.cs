using System;

using Graffiti.Core;

namespace DnugLeipzig.Plugins.Repositories
{
	public class PostRepository : IPostRepository
	{
		static readonly Data Data = new Data();

		#region IPostRepository Members
		public PostCollection GetPosts(string categoryName)
		{
			if (String.IsNullOrEmpty(categoryName))
			{
				throw new ArgumentOutOfRangeException("categoryName");
			}

			return Data.PostsByCategory(categoryName, int.MaxValue);
		}

		public void Save(Post post)
		{
			if (post == null)
			{
				throw new ArgumentNullException("post");
			}

			post.Save();
		}
		#endregion
	}
}