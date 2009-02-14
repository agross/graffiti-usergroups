using System;
using System.Collections.Generic;

namespace DnugLeipzig.Runtime.Plugins.Migration
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