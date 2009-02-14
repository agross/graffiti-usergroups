using System;
using System.Collections.Generic;

using DnugLeipzig.Definitions.Repositories;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Repositories
{
	public class PostRepository : IPostRepository
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

		public virtual void Save(Post instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}

			instance.Save();
		}

		public virtual IList<Post> GetByCategory(string categoryName)
		{
			return Data.PostsByCategory(categoryName, int.MaxValue);
		}

		public string GetCategoryName(Post post)
		{
			if (post == null)
			{
				throw new ArgumentNullException("post");
			}

			return post.Category.Name;
		}

		public Post GetByName(string postName)
		{
			if (String.IsNullOrEmpty(postName))
			{
				throw new ArgumentOutOfRangeException("postName");
			}

			return GraffitiData.GetPost(postName);
		}
		#endregion
	}
}