using System.Web;
using System.Web.Script.Serialization;

using DnugLeipzig.Definitions;

namespace DnugLeipzig.Runtime.Services
{
	internal class SingleEventRegistrationResult : IHttpResponse
	{
		public SingleEventRegistrationResult(int subscribedEvent)
		{
			SubscribedEvent = subscribedEvent;
		}

		#region Implementation of IHttpResponse
		public void Render(HttpResponse response)
		{
			response.ContentType = "application/json";
			response.Write(new JavaScriptSerializer().Serialize(this));
			response.End();
		}
		#endregion

		public int SubscribedEvent
		{
			get;
			protected set;
		}

		public bool IsOnWaitingList
		{
			get;
			set;
		}
	}
}