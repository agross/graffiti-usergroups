using System;
using System.Linq.Expressions;

namespace DnugLeipzig.Runtime.Mapping
{
	public abstract class TypeMapper<TSource, TResult> : Mapper<TSource, TResult>
	{
		public TypeMappingStep<TSource, TFrom, TResult> From<TFrom>(Func<TSource, TFrom> source)
		{
			return new TypeMappingStep<TSource, TFrom, TResult>(this, source);
		}

		#region Nested type: TypeMappingStep
		public class TypeMappingStep<TSourceType, TFrom, TResultType>
		{
			readonly TypeMapper<TSourceType, TResultType> _mapper;
			readonly Func<TSourceType, TFrom> _source;

			public TypeMappingStep(TypeMapper<TSourceType, TResultType> mapper, Func<TSourceType, TFrom> source)
			{
				_mapper = mapper;
				_source = source;
			}

			public void AutoConvertTo<TTo>(Expression<Func<TResultType, TTo>> result)
			{
				_mapper.Add(new TypeMapperStep<TSourceType, TFrom, TResultType, TTo>(_source, result));
			}

			public void To(Expression<Func<TResultType, TFrom>> result)
			{
				_mapper.Add(new TypeMapperStep<TSourceType, TFrom, TResultType, TFrom>(_source, result));
			}
		}
		#endregion
	}
}