using System;
using System.Collections.Generic;

using DnugLeipzig.Extensions.Extensions;

using Graffiti.Core;

namespace DnugLeipzig.Extensions
{
	internal class PostComparer : IComparer<Post>
	{
		readonly string DateFieldName;

		public PostComparer(string dateFieldName)
		{
			DateFieldName = dateFieldName;
		}

		#region IComparer<Post> Members
		public int Compare(Post x, Post y)
		{
			// Posts without date are shown at the top (DateTime.MaxValue).
			DateTime xDate = x.Custom(DateFieldName).AsEventDate();
			DateTime yDate = y.Custom(DateFieldName).AsEventDate();

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