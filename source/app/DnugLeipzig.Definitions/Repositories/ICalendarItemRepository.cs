using System.Collections.Generic;

using Graffiti.Core;

namespace DnugLeipzig.Definitions.Repositories
{
	public interface ICalendarItemRepository
	{
		ICalendar CreateCalendar(IEnumerable<Post> posts);
		ICalendar CreateCalendarForEvent(Post post);
	}
}