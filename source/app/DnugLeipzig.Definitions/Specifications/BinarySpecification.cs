using System;
using System.Linq;
using System.Linq.Expressions;

namespace DnugLeipzig.Definitions.Specifications
{
	public abstract class BinarySpecification<T> : Specification<T>
	{
		readonly ExpressionType _expressionType;
		readonly ISpecification<T> _left;
		readonly ISpecification<T> _right;

		protected BinarySpecification(ISpecification<T> left, ISpecification<T> right, ExpressionType expressionType)
		{
			_left = left;
			_expressionType = expressionType;
			_right = right;
		}

		public override Expression<Func<T, bool>> Predicate
		{
			get
			{
				var right = _right.Predicate;
				var left = _left.Predicate;

				var rightInvoke = Expression.Invoke(right, left.Parameters.Cast<Expression>());
				var expression = Expression.MakeBinary(_expressionType, left.Body, rightInvoke);

				return Expression.Lambda<Func<T, bool>>(expression, left.Parameters);
			}
		}
	}
}