using System.Diagnostics;

namespace DnugLeipzig.Definitions.Builders
{
	[DebuggerStepThrough]
	public abstract class EntityBuilder<TEntity>
	{
		public static implicit operator TEntity(EntityBuilder<TEntity> mother)
		{
			return mother.BuildInstance();
		}

		protected abstract TEntity BuildInstance();

		public TEntity Cast()
		{
			return this;
		}
	}
}