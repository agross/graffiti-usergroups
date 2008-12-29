using System.Web;

namespace DnugLeipzig.Definitions.Commands.Results
{
	public class NotFoundResult : ICommandResult
	{
		#region Implementation of ICommandResult
		public void Render(HttpResponse response)
		{
			response.Clear();
			response.StatusCode = 404;
			response.End();
		}
		#endregion
	}
}