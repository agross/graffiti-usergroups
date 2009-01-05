using System;
using System.Linq.Expressions;

namespace DnugLeipzig.Definitions.Specifications
{
	public class ExpressionSpecification<T> : Specification<T>
	{
		readonly Expression<Func<T, bool>> _predicate;

		public ExpressionSpecification(Expression<Func<T, bool>> predicate)
		{
			_predicate = predicate;
		}

		public override Expression<Func<T, bool>> Predicate
		{
			get { return _predicate; }
		}
	}
}