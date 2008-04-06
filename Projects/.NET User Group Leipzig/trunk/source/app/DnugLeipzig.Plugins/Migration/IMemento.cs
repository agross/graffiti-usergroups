using System;
using System.Collections.Generic;

namespace DnugLeipzig.Plugins.Migration
{
	interface IMemento
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