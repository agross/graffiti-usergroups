using System.Web;
using System.Web.Script.Serialization;

using DnugLeipzig.Definitions.Commands;

namespace DnugLeipzig.Runtime.Services
{
	internal class SingleEventRegistrationResult : ICommandResult
	{
		public SingleEventRegistrationResult(int subscribedEvent)
		{
			SubscribedEvent = subscribedEvent;
		}

		#region Implementation of ICommandResult
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