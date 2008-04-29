using System.Collections.Generic;

using Graffiti.Core;

namespace DnugLeipzig.Definitions.Repositories
{
	/// <summary>
	/// A repository that stores objects that supports categories.
	/// </summary>
	public interface ICategoryEnabledRepository : IPostRepository
	{
		IList<Post> GetAll();
		Category GetCategory();
	}
}