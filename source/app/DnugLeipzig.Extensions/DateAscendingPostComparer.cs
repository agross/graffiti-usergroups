using System;
using System.Collections.Generic;

using DnugLeipzig.Extensions.Extensions;

using Graffiti.Core;

namespace DnugLeipzig.Extensions
{
	internal class DateAscendingPostComparer : IComparer<Post>
	{
		readonly string DateFieldName;

		public DateAscendingPostComparer(string dateFieldName)
		{
			DateFieldName = dateFieldName;
		}

		#region IComparer<Post> Members
		public int Compare(Post x, Post y)
		{
			// Posts without date are shown at the top (DateTime.MinValue).
			DateTime xDate = x[DateFieldName].AsEventDate() == DateTime.MaxValue
			                 	? DateTime.MinValue
			                 	: x[DateFieldName].AsEventDate();
			DateTime yDate = y[DateFieldName].AsEventDate() == DateTime.MaxValue
			                 	? DateTime.MinValue
			                 	: y[DateFieldName].AsEventDate();

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