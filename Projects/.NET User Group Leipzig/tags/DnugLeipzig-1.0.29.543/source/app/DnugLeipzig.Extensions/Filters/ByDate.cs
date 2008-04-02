using System;
using System.Collections.Generic;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Filters
{
	internal abstract class ByDate : IPostFilter
	{
		protected string _dateFieldName;

		protected ByDate(string dateFieldName)
		{
			if (String.IsNullOrEmpty(dateFieldName))
			{
				throw new ArgumentOutOfRangeException("dateFieldName");
			}

			_dateFieldName = dateFieldName;
		}

		#region IPostFilter Members
		public abstract List<Post> Execute(List<Post> posts);
		#endregion
	}
}