using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

using DnugLeipzig.Definitions.Repositories;

namespace DnugLeipzig.Runtime.Repositories
{
	public class Calendar : ICalendar
	{
		readonly string _calendarName;
		readonly bool _generateSubscriptionHeader;

		public Calendar(string calendarName, bool generateSubscriptionHeader)
		{
			_calendarName = calendarName;
			_generateSubscriptionHeader = generateSubscriptionHeader;
			Items = new List<ICalendarItem>();
		}

		#region ICalendar Members
		public void Render(HttpResponse response)
		{
			string serializedItems = ToString();

			response.Clear();
			var calendarName = _calendarName.Replace(',', ' ');
			response.AppendHeader("Content-Disposition",
			                      String.Format("attachment; filename={0}.ics",
			                                    HttpUtility.UrlPathEncode(calendarName)));
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
			if (_generateSubscriptionHeader)
			{
				result.AppendLine("METHOD:PUBLISH");
				result.AppendLine("X-PUBLISHED-TTL:PT60M");
				result.Append("X-WR-CALNAME:");
				result.AppendLine(_calendarName);
			}

			foreach (var item in Items)
			{
				try
				{
					result.Append(item.ToString());
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