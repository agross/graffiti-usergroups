using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;

using DnugLeipzig.Definitions;

namespace DnugLeipzig.Runtime.Services
{
	internal class MultipleEventRegistrationResult : IHttpResponse
	{
		public MultipleEventRegistrationResult()
		{
			EventResults = new List<IHttpResponse>();
		}

		#region Implementation of IHttpResponse
		public void Render(HttpResponse response)
		{
			response.ContentType = "application/json";
			response.Write(new JavaScriptSerializer().Serialize(this));
			response.End();
		}
		#endregion

		public ICollection<IHttpResponse> EventResults
		{
			get;
			protected set;
		}
	}
}