using System;
using System.Text;
using System.Web;

using DnugLeipzig.Definitions.Repositories;

namespace DnugLeipzig.Runtime.Repositories
{
	public class CalendarItem : ICalendarItem
	{
		#region ICalendarItem Members
		public DateTime StartDate
		{
			get;
			set;
		}

		public DateTime EndDate
		{
			get;
			set;
		}

		public string Location
		{
			get;
			set;
		}

		public string Subject
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public string Categories
		{
			get;
			set;
		}

		public DateTime LastModified
		{
			get;
			set;
		}

		public override string ToString()
		{
			if (!IsValid())
			{
				throw new InvalidOperationException("The calendar item is not valid.");
			}

			var builder = new StringBuilder();

			// Header.
			builder.AppendLine("BEGIN:VEVENT");

			// Fill in data.
			builder.AppendFormat("DTSTART:{0:yyyy}{0:MM}{0:dd}T{0:HH}{0:mm}{0:ss}Z{1}",
			                     StartDate.ToUniversalTime(),
			                     Environment.NewLine);
			DateTime endDate = EndDate;
			if (endDate == DateTime.MaxValue)
			{
				endDate = StartDate;
			}
			builder.AppendFormat("DTEND:{0:yyyy}{0:MM}{0:dd}T{0:HH}{0:mm}{0:ss}Z{1}",
			                     endDate.ToUniversalTime(),
			                     Environment.NewLine);
			builder.AppendFormat("DTSTAMP:{0:yyyy}{0:MM}{0:dd}T{0:HH}{0:mm}{0:ss}Z{1}",
			                     DateTime.Now.ToUniversalTime(),
			                     Environment.NewLine);
			builder.AppendFormat("LAST-MODIFIED:{0:yyyy}{0:MM}{0:dd}T{0:HH}{0:mm}{0:ss}Z{1}",
			                     LastModified.ToUniversalTime(),
			                     Environment.NewLine);
			builder.AppendFormat("LOCATION:{0}{1}", Location, Environment.NewLine);
			builder.AppendFormat("CATEGORIES:{0}{1}", Categories, Environment.NewLine);
			builder.AppendLine("CLASS:PUBLIC");
			builder.AppendFormat("DESCRIPTION:{0}{1}", Description, Environment.NewLine);
			builder.AppendFormat("SUMMARY:{0}{1}", Subject, Environment.NewLine);

			// Footer.
			builder.AppendLine("END:VEVENT");
			
			return builder.ToString();
		}

		public void Render(HttpResponse response)
		{
			string serializedItem = ToString();

			response.Clear();
			response.AppendHeader("Content-Disposition",
								  String.Format("attachment; filename={0}.ics",
												HttpUtility.UrlPathEncode(Subject)));
			response.AppendHeader("Content-Length", serializedItem.Length.ToString());
			response.ContentType = "text/calendar";

			response.Write(serializedItem);
		}
		#endregion

		/// <summary>
		/// Checks if the minimum requirements are fulfilled to create a valid iCalendar item.
		/// </summary>
		public bool IsValid()
		{
			if (StartDate == DateTime.MaxValue)
			{
				return false;
			}

			if (EndDate <= StartDate)
			{
				return false;
			}

			return true;
		}
	}
}