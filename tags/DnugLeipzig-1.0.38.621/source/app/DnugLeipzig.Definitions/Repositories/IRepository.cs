using System.Collections.Generic;

using Graffiti.Core;

namespace DnugLeipzig.Definitions.Repositories
{
	public interface IRepository<T> where T : class
	{
		Data GraffitiData
		{
			get;
		}

		T GetById(int id);
		void Save(T instance);
	}
}