using System.Web;

namespace DnugLeipzig.Definitions.Commands.Results
{
	public class ForbiddenResult : IHttpResponse
	{
		#region Implementation of IHttpResponse
		public void Render(HttpResponse response)
		{
			response.Clear();
			response.StatusCode = 403;
			response.End();
		}
		#endregion
	}
}