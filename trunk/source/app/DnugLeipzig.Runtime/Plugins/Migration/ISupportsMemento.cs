using DnugLeipzig.Runtime.Plugins.Migration;

namespace DnugLeipzig.Runtime.Plugins.Migration
{
	public interface ISupportsMemento
	{
		IMemento CreateMemento();
	}
}