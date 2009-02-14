using System;
using System.Collections.Generic;

using DnugLeipzig.Definitions.Extensions;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Macros.PostComparer
{
	internal class DateDescendingPostComparer : IComparer<Post>
	{
		readonly string _dateFieldName;

		public DateDescendingPostComparer(string dateFieldName)
		{
			_dateFieldName = dateFieldName;
		}

		#region IComparer<Post> Members
		public int Compare(Post x, Post y)
		{
			// Posts without date are shown at the top (DateTime.MaxValue).
			DateTime xDate = x[_dateFieldName].AsEventDate();
			DateTime yDate = y[_dateFieldName].AsEventDate();

			int dateResult = xDate.CompareTo(yDate) * -1;

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