using System;
using System.Web;

using DnugLeipzig.Definitions.Commands;

namespace DnugLeipzig.Runtime.Services
{
	internal class CalendarItemResult : ICommandResult
	{
		readonly CalendarItemService.CalendarItem _calendarItem;

		public CalendarItemResult(CalendarItemService.CalendarItem calendarItem)
		{
			_calendarItem = calendarItem;
		}

		#region Implementation of ICommandResult
		public void Render(HttpResponse response)
		{
			string serializedItem = _calendarItem.ToString();

			response.Clear();
			response.AppendHeader("Content-Disposition",
			                      String.Format("attachment; filename={0}.ics",
			                                    HttpUtility.UrlPathEncode(_calendarItem.Subject)));
			response.AppendHeader("Content-Length", serializedItem.Length.ToString());
			response.ContentType = "text/calendar";
			response.Write(serializedItem);
		}
		#endregion
	}
}