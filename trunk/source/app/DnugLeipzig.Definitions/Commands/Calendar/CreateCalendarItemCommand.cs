using DnugLeipzig.Definitions.Services;

namespace DnugLeipzig.Definitions.Commands.Calendar
{
	public class CreateCalendarItemCommand : ICommand
	{
		public CreateCalendarItemCommand(int eventId)
		{
			EventId = eventId;
		}

		public int EventId
		{
			get;
			protected set;
		}

		#region Implementation of ICommand
		public ICommandResult Execute()
		{
			return IoC.Resolve<ICalendarItemService>().CreateCalendarItem(this);
		}
		#endregion
	}
}