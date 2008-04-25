using System;

using Graffiti.Core;

namespace DnugLeipzig.Plugins.Repositories
{
	public interface IPostRepository
	{
		PostCollection GetPosts(string categoryName);
		void Save(Post post);
	}
}