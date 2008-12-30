using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Definitions.Commands.Calendar;
using DnugLeipzig.Definitions.Services;

namespace DnugLeipzig.Runtime.Commands.Calendar
{
	public class CreateCalendarItemCommand : ICreateCalendarItemCommand
	{
		readonly ICalendarItemService _calendarItemService;

		public CreateCalendarItemCommand(ICalendarItemService calendarItemService)
		{
			_calendarItemService = calendarItemService;
		}

		#region ICreateCalendarItemCommand Members
		public int EventId
		{
			get;
			protected set;
		}
		#endregion

		#region Implementation of ICommand
		public ICommandResult Execute()
		{
			return _calendarItemService.CreateCalendarItem(this);
		}
		#endregion
	}
}