using System;
using System.Linq.Expressions;
using System.Reflection;

using DnugLeipzig.Definitions.Mapping;

namespace DnugLeipzig.Runtime.Mapping
{
	public class ClassMapperStep<TSource, TFromType, TResult, TResultType> : IMapperStep<TSource, TResult>
	{
		readonly Func<TSource, TFromType> _from;
		readonly MethodInfo _propertySetter;

		public ClassMapperStep(Func<TSource, TFromType> from, Expression<Func<TResult, TResultType>> to)
		{
			_from = from;
			_propertySetter = GetSetter(to);
		}

		#region IMapperStep<TSource,TResult> Members
		public void Map(TSource source, TResult result)
		{
			_propertySetter.Invoke(result, new[] { Convert(_from(source)) });
		}
		#endregion

		static object Convert(TFromType value)
		{
			if (typeof(TResultType) != typeof(TFromType))
			{
				return (TResultType) System.Convert.ChangeType(value, typeof(TResultType));
			}
			return value;
		}

		static MethodInfo GetSetter(Expression<Func<TResult, TResultType>> to)
		{
			MemberExpression body = (MemberExpression) to.Body;
			var propertyInfo = body.Member.ReflectedType.GetProperty(body.Member.Name);

			return propertyInfo.GetSetMethod(true);
		}
	}
}