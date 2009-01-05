using System;
using System.Linq.Expressions;

using DnugLeipzig.Definitions.Mapping;
using DnugLeipzig.Definitions.Validation;

namespace DnugLeipzig.Runtime.Validation
{
	public class ValidationStep<TSource> : IMapperStep<TSource, NotificationResult>
	{
		readonly Expression<Func<TSource, bool>> _condition;
		readonly INotification _notification;

		public ValidationStep(Expression<Func<TSource, bool>> condition, INotification notification)
		{
			_condition = condition;
			_notification = notification;
		}

		#region Implementation of IMapperStep<TSource,TResult>
		public void Map(TSource from, NotificationResult to)
		{
			if (_condition.Compile().Invoke(from))
			{
				to.Add(_notification);
			}
		}
		#endregion
	}
}