using System.Web;

namespace DnugLeipzig.Definitions
{
	public interface IHttpResponse
	{
		void Render(HttpResponse response);
	}
}