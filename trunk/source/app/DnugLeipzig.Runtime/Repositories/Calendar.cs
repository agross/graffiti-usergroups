using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

using DnugLeipzig.Definitions.GraffitiIntegration;
using DnugLeipzig.Definitions.Repositories;

namespace DnugLeipzig.Runtime.Repositories
{
	public class Calendar : ICalendar
	{
		readonly IGraffitiSiteSettings _settings;

		public Calendar(IGraffitiSiteSettings settings)
		{
			_settings = settings;
			Items = new List<ICalendarItem>();
		}

		#region ICalendar Members
		public void Render(HttpResponse response)
		{
			string serializedItems = ToString();

			response.Clear();
			response.AppendHeader("Content-Disposition",
								  String.Format("attachment; filename={0}.ics",
												HttpUtility.UrlPathEncode(_settings.Title)));
			response.AppendHeader("Content-Length", serializedItems.Length.ToString());
			response.ContentType = "text/calendar";

			response.Write(serializedItems);
		}

		public ICollection<ICalendarItem> Items
		{
			get;
			private set;
		}
		#endregion

		public override string ToString()
		{
			StringBuilder result = new StringBuilder();

			// Header.
			result.AppendLine("BEGIN:VCALENDAR");
			result.AppendLine("PRODID:-//Microsoft Corporation//Outlook MIMEDIR//EN");
			result.AppendLine("VERSION:1.0");
			result.AppendLine("METHOD:PUBLISH");
			result.AppendLine("X-PUBLISHED-TTL:PT60M");
			result.Append("X-WR-CALNAME:");
			result.AppendLine(_settings.Title);
			
			foreach (var item in Items)
			{
				try
				{
					result.AppendLine(item.ToString());
				}
				catch (InvalidOperationException)
				{
					// This item is invalid.
				}
			}

			result.AppendLine("END:VCALENDAR");

			return result.ToString();
		}
	}
}