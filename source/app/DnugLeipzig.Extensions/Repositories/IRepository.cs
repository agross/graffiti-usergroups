using System.Collections.Generic;

using DnugLeipzig.Extensions.Filters;

namespace DnugLeipzig.Extensions.Repositories
{
	public interface IRepository<T> where T : class
	{
		List<T> Get(params IPostFilter[] filters);

		T Get(int id);
	}
}