namespace DnugLeipzig.Definitions.Mapping
{
	public interface IMapperStep<TSource, TResult>
	{
		void Map(TSource from, TResult to);
	}
}