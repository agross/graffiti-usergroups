using System;
using System.Collections.Generic;

namespace DnugLeipzig.Plugins.Migration
{
	public interface IMemento
	{
		string CategoryName
		{
			get;
		}

		IDictionary<Guid, FieldInfo> Fields
		{
			get;
		}
	}
}