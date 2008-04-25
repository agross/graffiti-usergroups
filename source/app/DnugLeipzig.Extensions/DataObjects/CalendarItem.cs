using System;
using System.Text;

namespace DnugLeipzig.Extensions.DataObjects
{
	public class CalendarItem
	{
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

		public override string ToString()
		{
			var builder = new StringBuilder();

			// Header.
			builder.AppendLine("BEGIN:VCALENDAR");
			builder.AppendLine("PRODID:-//Microsoft Corporation//Outlook MIMEDIR//EN");
			builder.AppendLine("VERSION:1.0");
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
			builder.AppendLine("END:VCALENDAR");

			return builder.ToString();
		}
	}
}