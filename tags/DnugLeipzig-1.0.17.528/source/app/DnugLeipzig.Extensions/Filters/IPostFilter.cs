using System.Collections.Generic;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Filters
{
	public interface IPostFilter
	{
		List<Post> Execute(List<Post> posts);
	}
}