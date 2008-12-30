using System;
using System.Collections.Generic;

using DnugLeipzig.Definitions.Repositories;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Repositories
{
	public class PostRepository : Repository, IPostRepository
	{
		protected static readonly Data Data = new Data();

		#region IPostRepository Members
		public Data GraffitiData
		{
			get { return Data; }
		}

		public virtual Post GetById(int id)
		{
			return Data.GetPost(id);
		}

		public virtual void Save(Post post)
		{
			if (post == null)
			{
				throw new ArgumentNullException("post");
			}

			post.Save();
		}

		public virtual IList<Post> GetByCategory(string categoryName)
		{
			return Data.PostsByCategory(categoryName, int.MaxValue);
		}

		public string GetCategoryNameOf(Post post)
		{
			if (post == null)
			{
				throw new ArgumentNullException("post");
			}

			return post.Category.Name;
		}

		public Post GetByTitle(string title)
		{
			if (String.IsNullOrEmpty(title))
			{
				throw new ArgumentOutOfRangeException("title");
			}

			return GraffitiData.GetPost(title);
		}
		#endregion
	}
}