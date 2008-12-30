namespace DnugLeipzig.Definitions.Commands
{
	public interface ICommand
	{
		IHttpResponse Execute();
	}
}