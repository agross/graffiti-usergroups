using System.Linq.Expressions;

namespace DnugLeipzig.Definitions.Specifications
{
	public class AndSpecification<T> : BinarySpecification<T>
	{
		public AndSpecification(ISpecification<T> left, ISpecification<T> right)
			: base(left, right, ExpressionType.AndAlso)
		{
		}
	}
}