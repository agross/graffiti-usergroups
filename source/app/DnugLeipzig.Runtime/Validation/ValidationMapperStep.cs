using System;

using DnugLeipzig.Definitions.Mapping;
using DnugLeipzig.Definitions.Validation;

namespace DnugLeipzig.Runtime.Validation
{
	public class ValidationMapperStep<TSource> : IMapperStep<TSource, ValidationReport>
	{
		readonly Func<TSource, bool> _condition;
		readonly Func<TSource, INotification> _notification;

		public ValidationMapperStep(Func<TSource, bool> condition, INotification notification)
		{
			_condition = condition;
			_notification = x => notification;
		}

		public ValidationMapperStep(Func<TSource, bool> condition, Func<TSource, INotification> notification)
		{
			_condition = condition;
			_notification = notification;
		}

		#region Implementation of IMapperStep<TSource,TResult>
		public void Map(TSource source, ValidationReport report)
		{
			if (_condition.Invoke(source))
			{
				report.Add(_notification.Invoke(source));
			}
		}
		#endregion
	}
}