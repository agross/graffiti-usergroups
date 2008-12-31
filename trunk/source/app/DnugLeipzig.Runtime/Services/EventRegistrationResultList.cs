using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;

using DnugLeipzig.Definitions.Services;

namespace DnugLeipzig.Runtime.Services
{
	public class EventRegistrationResultList : List<IEventRegistrationResult>, IEventRegistrationResultList
	{
		#region Implementation of IHttpResponse
		public void Render(HttpResponse response)
		{
			response.ContentType = "application/json";
			response.Write(new JavaScriptSerializer().Serialize(this));
		}
		#endregion
	}
}