using System.Collections.Generic;

using Graffiti.Core;

namespace DnugLeipzig.Definitions.Repositories
{
	public interface IPostRepository : IRepository<Post>
	{
		IList<Post> GetByCategory(string categoryName);
	}
}