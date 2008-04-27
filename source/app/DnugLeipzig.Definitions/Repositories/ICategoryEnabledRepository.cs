using System.Collections.Generic;

using Graffiti.Core;

namespace DnugLeipzig.Definitions.Repositories
{
	public interface ICategoryEnabledRepository : IPostRepository
	{
		IList<Post> GetAll();
		Category GetCategory();
	}
}