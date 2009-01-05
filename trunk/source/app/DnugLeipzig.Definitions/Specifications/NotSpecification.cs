using System;
using System.Linq;
using System.Linq.Expressions;

namespace DnugLeipzig.Definitions.Specifications
{
	public class NotSpecification<T> : Specification<T>
	{
		readonly ISpecification<T> _original;

		public NotSpecification(ISpecification<T> original)
		{
			_original = original;
		}

		public override Expression<Func<T, bool>> Predicate
		{
			get
			{
				var predicate = _original.Predicate;
				var invoke = Expression.Invoke(predicate, predicate.Parameters.Cast<Expression>());
				var expression = Expression.Not(invoke);

				return Expression.Lambda<Func<T, bool>>(expression, predicate.Parameters);
			}
		}
	}
}