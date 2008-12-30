using System.Web;

namespace DnugLeipzig.Definitions.Commands.Results
{
	public class NotFoundResult : IHttpResponse
	{
		#region Implementation of IHttpResponse
		public void Render(HttpResponse response)
		{
			response.Clear();
			response.StatusCode = 404;
			response.End();
		}
		#endregion
	}
}