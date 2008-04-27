using System.Threading;
using System.Web;

namespace DnugLeipzig.DemoSite.Handlers
{
	public class DemoSiteHandler:IHttpHandler
	{
		#region IHttpHandler Members
		public void ProcessRequest(HttpContext context)
		{
			Thread.Sleep(3000);
		}

		public bool IsReusable
		{
			get { return false; }
		}
		#endregion
	}
}