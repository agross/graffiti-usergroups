using System;
using System.Web;

namespace DnugLeipzig.Extensions.Handlers
{
	public class RegistrationHandler : IHttpHandler
	{
		#region IHttpHandler Members
		public void ProcessRequest(HttpContext context)
		{
			throw new NotImplementedException();
		}

		public bool IsReusable
		{
			get { return true; }
		}
		#endregion
	}
}