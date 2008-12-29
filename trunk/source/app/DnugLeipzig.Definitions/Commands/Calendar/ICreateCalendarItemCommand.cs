namespace DnugLeipzig.Definitions.Commands.Calendar
{
	public interface ICreateCalendarItemCommand:ICommand
	{
		int EventId
		{
			get;
		}
	}
}