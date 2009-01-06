using System.Collections.Generic;

using DnugLeipzig.Definitions.Mapping;

namespace DnugLeipzig.Runtime.Mapping
{
	public abstract class Mapper<TSource, TResult> : IMapper<TSource, TResult>
	{
		readonly IList<IMapperStep<TSource, TResult>> _mappingSteps = new List<IMapperStep<TSource, TResult>>();

		#region IMapper<TSource,TResult> Members
		public void Map(TSource from, TResult to)
		{
			foreach (var mappingStep in _mappingSteps)
			{
				mappingStep.Map(from, to);
			}
		}

		public void Add(IMapperStep<TSource, TResult> mapperStep)
		{
			_mappingSteps.Add(mapperStep);
		}
		#endregion
	}
}