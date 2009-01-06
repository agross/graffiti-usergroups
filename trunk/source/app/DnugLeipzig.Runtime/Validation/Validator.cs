using System;
using System.Linq.Expressions;

using DnugLeipzig.Definitions.Specifications;
using DnugLeipzig.Definitions.Validation;
using DnugLeipzig.Runtime.Mapping;

namespace DnugLeipzig.Runtime.Validation
{
	public abstract class Validator<T> : Mapper<T, ValidationReport>, IValidator<T>
	{
		#region Implementation of IValidator<T>
		public ValidationReport Validate(T instance)
		{
			ValidationReport report = new ValidationReport();

			Map(instance, report);

			return report;
		}
		#endregion

		public ValidatingStep If(Func<T, bool> condition)
		{
			return new ValidatingStep(this, condition);
		}

		public ValidatingStep IfNot(Expression<Func<T, bool>> condition)
		{
			return new ValidatingStep(this, new ExpressionSpecification<T>(condition).Negate().Predicate.Compile());
		}

		#region Nested type: ValidatingStep
		public class ValidatingStep
		{
			readonly Func<T, bool> _condition;
			readonly Validator<T> _validator;

			public ValidatingStep(Validator<T> validator, Func<T, bool> condition)
			{
				_validator = validator;
				_condition = condition;
			}

			public void AddNotification(INotification notification)
			{
				_validator.Add(new ValidationMapperStep<T>(_condition, notification));
			}

			public void AddNotification(Func<T, INotification> notification)
			{
				_validator.Add(new ValidationMapperStep<T>(_condition, notification));
			}
		}
		#endregion
	}
}