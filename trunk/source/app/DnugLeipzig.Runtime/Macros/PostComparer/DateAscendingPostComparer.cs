using System;
using System.Collections.Generic;

using DnugLeipzig.Definitions.Extensions;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Macros.PostComparer
{
	internal class DateAscendingPostComparer : IComparer<Post>
	{
		readonly string _dateFieldName;

		public DateAscendingPostComparer(string dateFieldName)
		{
			_dateFieldName = dateFieldName;
		}

		#region IComparer<Post> Members
		public int Compare(Post x, Post y)
		{
			// Posts without date are shown at the top (DateTime.MinValue).
			DateTime xDate = x[_dateFieldName].AsEventDate() == DateTime.MaxValue
			                 	? DateTime.MinValue
			                 	: x[_dateFieldName].AsEventDate();
			DateTime yDate = y[_dateFieldName].AsEventDate() == DateTime.MaxValue
			                 	? DateTime.MinValue
			                 	: y[_dateFieldName].AsEventDate();

			int dateResult = xDate.CompareTo(yDate);

			if (dateResult == 0)
			{
				// Dates equal, compare by title.
				return String.Compare(x.Title, y.Title, StringComparison.CurrentCultureIgnoreCase);
			}

			return dateResult;
		}
		#endregion
	}
}