using System.Linq;

namespace DnugLeipzig.Definitions.Specifications
{
	public static class SpecificationExtensions
	{
		public static IQueryable<T> Filter<T>(this ISpecification<T> specification, IQueryable<T> values)
		{
			return values.Where(specification.Predicate);
		}

		public static bool IsSatisfiedBy<T>(this ISpecification<T> specification, T value)
		{
			return specification.Predicate.Compile().Invoke(value);
		}

		public static ISpecification<T> And<T>(this ISpecification<T> left, ISpecification<T> right)
		{
			return new AndSpecification<T>(left, right);
		}

		public static ISpecification<T> Or<T>(this ISpecification<T> left, ISpecification<T> right)
		{
			return new OrSpecification<T>(left, right);
		}

		public static ISpecification<T> Negate<T>(this ISpecification<T> specification)
		{
			return new NotSpecification<T>(specification);
		}
	}
}