namespace DnugLeipzig.Definitions.Mapping
{
	public interface IMapperStep<TSource, TResult>
	{
		void Map(TSource source, TResult result);
	}
}