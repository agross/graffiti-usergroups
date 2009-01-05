using System;
using System.Linq.Expressions;

using DnugLeipzig.Definitions.Mapping;
using DnugLeipzig.Definitions.Specifications;
using DnugLeipzig.Definitions.Validation;
using DnugLeipzig.Runtime.Mapping;

namespace DnugLeipzig.Runtime.Validation
{
	public abstract class Validator<T> : IValidator<T>
	{
		readonly IMapper<T, NotificationResult> _mapper = new Mapper<T, NotificationResult>();

		protected internal IMapper<T, NotificationResult> Mapper
		{
			get { return _mapper; }
		}

		#region Implementation of IValidator<T>
		public NotificationResult Validate(T instance)
		{
			NotificationResult result = new NotificationResult();

			_mapper.Map(instance, result);

			return result;
		}
		#endregion

		public ValidatingStep If(Expression<Func<T, bool>> condition)
		{
			return new ValidatingStep(this, condition);
		}

		public ValidatingStep IfNot(Expression<Func<T, bool>> condition)
		{
			return new ValidatingStep(this, new ExpressionSpecification<T>(condition).Negate().Predicate);
		}

		#region Nested type: ValidatingStep
		public class ValidatingStep
		{
			readonly Expression<Func<T, bool>> _condition;
			readonly Validator<T> _validator;

			public ValidatingStep(Validator<T> validator, Expression<Func<T, bool>> condition)
			{
				_validator = validator;
				_condition = condition;
			}

			public void AddNotification(INotification notification)
			{
				_validator.Mapper.Add(new ValidationStep<T>(_condition, notification));
			}
		}
		#endregion
	}
}