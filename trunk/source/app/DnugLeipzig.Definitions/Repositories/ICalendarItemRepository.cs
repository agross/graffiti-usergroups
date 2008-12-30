using Graffiti.Core;

namespace DnugLeipzig.Definitions.Repositories
{
	public interface ICalendarItemRepository
	{
		ICalendarItem CreateCalendarItemForEvent(Post post);
	}
}