using System.Web;

namespace DnugLeipzig.Definitions.Commands.Results
{
	public class ErrorResult : IHttpResponse
	{
		#region Implementation of IHttpResponse
		public void Render(HttpResponse response)
		{
			response.Clear();
			response.StatusCode = 500;
			response.End();
		}
		#endregion
	}
}