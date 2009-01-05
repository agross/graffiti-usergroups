using System;
using System.Collections.Generic;

using DnugLeipzig.Runtime.Plugins.Migration;

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