using System;
using System.Linq.Expressions;

namespace DnugLeipzig.Definitions.Specifications
{
	public interface ISpecification<T>
	{
		Expression<Func<T, bool>> Predicate
		{
			get;
		}
	}
}