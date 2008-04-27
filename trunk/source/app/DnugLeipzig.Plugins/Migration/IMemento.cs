using System.Collections.Generic;

namespace DnugLeipzig.Plugins.Migration
{
	public interface IMemento
	{
		string CategoryName
		{
			get;
		}

		Dictionary<string, FieldInfo> Fields
		{
			get;
		}
	}
}