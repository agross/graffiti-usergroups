using System;

using Graffiti.Core;

namespace DnugLeipzig.Extensions
{
	[Chalk("dnugCollections")]
	public class DnugCollections
	{
		public void SortEvents(PostCollection posts, string pageType)
		{
			if (posts == null || posts.Count <= 1)
			{
				return;
			}

			// Only sort the category view.
			if (!string.Equals(pageType, "category", StringComparison.OrdinalIgnoreCase))
			{
				return;
			}

			// Ensure that all elements are events.
			if (!Array.TrueForAll(posts.ToArray(), Util.IsEvent))
			{
				// Return with default collation.
				return;
			}

			posts.Sort(delegate(Post x, Post y)
				{
					// Posts without date are shown at the top (DateTime.MaxValue).
					DateTime xDate;
					if (!DateTime.TryParse(x.CustomFields()["Datum"], out xDate))
					{
						xDate = DateTime.MaxValue;
					}

					DateTime yDate;
					if (!DateTime.TryParse(y.CustomFields()["Datum"], out yDate))
					{
						yDate = DateTime.MaxValue;
					}

					int dateResult = xDate.CompareTo(yDate) * -1;

					if (dateResult == 0)
					{
						// Dates equal, compary by title.
						return String.Compare(x.Title, y.Title, StringComparison.CurrentCultureIgnoreCase);
					}

					return dateResult;
				});
		}
	}
}