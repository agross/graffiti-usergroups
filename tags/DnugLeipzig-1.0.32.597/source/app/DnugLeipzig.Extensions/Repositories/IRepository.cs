using System.Collections.Generic;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Repositories
{
	public interface IRepository<T> where T : class
	{
		Data Data
		{
			get; 
		}

		IList<T> GetAll();
		T GetById(int id);
		Category GetCategory();
	}
}