using System.Web;

namespace DnugLeipzig.Definitions.Commands.Results
{
	public class ErrorResult : ICommandResult
	{
		#region Implementation of ICommandResult
		public void Render(HttpResponse response)
		{
			response.Clear();
			response.StatusCode = 500;
			response.End();
		}
		#endregion
	}
}