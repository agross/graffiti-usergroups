using System.Collections.Generic;

namespace DnugLeipzig.Definitions.Repositories
{
	public interface ICalendar : IHttpResponse
	{
		ICollection<ICalendarItem> Items
		{
			get;
		}
	}
}