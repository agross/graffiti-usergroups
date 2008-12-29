using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;

using DnugLeipzig.Definitions.Commands;

namespace DnugLeipzig.Runtime.Services
{
	internal class MultipleEventRegistrationResult : ICommandResult
	{
		public MultipleEventRegistrationResult()
		{
			EventResults = new List<ICommandResult>();
		}

		#region Implementation of ICommandResult
		public void Render(HttpResponse response)
		{
			response.ContentType = "application/json";
			response.Write(new JavaScriptSerializer().Serialize(this));
			response.End();
		}
		#endregion

		public ICollection<ICommandResult> EventResults
		{
			get;
			protected set;
		}
	}
}