using System.Web;

namespace DnugLeipzig.Definitions.Commands.Results
{
	public class ForbiddenResult : ICommandResult
	{
		#region Implementation of ICommandResult
		public void Render(HttpResponse response)
		{
			response.Clear();
			response.StatusCode = 403;
			response.End();
		}
		#endregion
	}
}