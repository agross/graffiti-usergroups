namespace DnugLeipzig.Definitions.Repositories
{
	public interface ICalendarItemRepository
	{
		ICalendarItem GetCalendarItemForEvent(int eventId);
	}
}