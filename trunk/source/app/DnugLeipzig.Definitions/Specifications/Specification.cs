using System;
using System.Linq.Expressions;

namespace DnugLeipzig.Definitions.Specifications
{
	public abstract class Specification<T> : ISpecification<T>
	{
		#region ISpecification<T> Members
		public abstract Expression<Func<T, bool>> Predicate
		{
			get;
		}
		#endregion
	}
}