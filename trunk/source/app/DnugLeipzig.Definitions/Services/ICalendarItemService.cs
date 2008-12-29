using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Definitions.Commands.Calendar;

namespace DnugLeipzig.Definitions.Services
{
	public interface ICalendarItemService
	{
		ICommandResult CreateCalendarItem(ICreateCalendarItemCommand command);
	}
}