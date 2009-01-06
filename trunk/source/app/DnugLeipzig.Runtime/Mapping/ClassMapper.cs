using System;
using System.Linq.Expressions;

namespace DnugLeipzig.Runtime.Mapping
{
	public abstract class ClassMapper<TSource, TResult> : Mapper<TSource, TResult>
	{
		public ClassMappingStep<TSource, TFrom, TResult> From<TFrom>(Func<TSource, TFrom> source)
		{
			return new ClassMappingStep<TSource, TFrom, TResult>(this, source);
		}

		#region Nested type: ClassMappingStep
		public class ClassMappingStep<TSourceType, TFrom, TResultType>
		{
			readonly ClassMapper<TSourceType, TResultType> _mapper;
			readonly Func<TSourceType, TFrom> _source;

			public ClassMappingStep(ClassMapper<TSourceType, TResultType> mapper, Func<TSourceType, TFrom> source)
			{
				_mapper = mapper;
				_source = source;
			}

			public void AutoConvertTo<TTo>(Expression<Func<TResultType, TTo>> result)
			{
				_mapper.Add(new ClassMapperStep<TSourceType, TFrom, TResultType, TTo>(_source, result));
			}

			public void To(Expression<Func<TResultType, TFrom>> result)
			{
				_mapper.Add(new ClassMapperStep<TSourceType, TFrom, TResultType, TFrom>(_source, result));
			}
		}
		#endregion
	}
}