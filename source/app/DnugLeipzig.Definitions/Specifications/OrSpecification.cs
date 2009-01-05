using System.Linq.Expressions;

namespace DnugLeipzig.Definitions.Specifications
{
	public class OrSpecification<T> : BinarySpecification<T>
	{
		public OrSpecification(ISpecification<T> left, ISpecification<T> right)
			: base(left, right, ExpressionType.OrElse)
		{
		}
	}
}