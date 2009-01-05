namespace DnugLeipzig.Definitions.Mapping
{
	public interface IMapper<TSource, TResult>
	{
		void Map(TSource from, TResult to);
		void Add(IMapperStep<TSource, TResult> mapperStep);
	}
}