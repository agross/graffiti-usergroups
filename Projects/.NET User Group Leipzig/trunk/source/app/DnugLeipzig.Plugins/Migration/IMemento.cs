using System.Collections.Generic;

namespace DnugLeipzig.Plugins.Migration
{
	internal interface IMemento
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