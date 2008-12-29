using System.Web;

namespace DnugLeipzig.Definitions.Commands
{
	public interface ICommandResult
	{
		void Render(HttpResponse response);
	}
}