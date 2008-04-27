using System;
using System.Collections.Generic;

using DnugLeipzig.Definitions.Extensions;

using Graffiti.Core;

namespace DnugLeipzig.Extensions
{
	internal class DateDescendingPostComparer : IComparer<Post>
	{
		readonly string DateFieldName;

		public DateDescendingPostComparer(string dateFieldName)
		{
			DateFieldName = dateFieldName;
		}

		#region IComparer<Post> Members
		public int Compare(Post x, Post y)
		{
			// Posts without date are shown at the top (DateTime.MaxValue).
			DateTime xDate = x[DateFieldName].AsEventDate();
			DateTime yDate = y[DateFieldName].AsEventDate();

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