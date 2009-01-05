using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;

using DnugLeipzig.Definitions.Validation;

namespace DnugLeipzig.Definitions.Commands.Results
{
	public class ValidationErrorResult : IHttpResponse
	{
		public ValidationErrorResult(IEnumerable<INotification> validationErrors)
		{
			ValidationErrors = validationErrors;
		}

		#region Implementation of IHttpResponse
		public void Render(HttpResponse response)
		{
			response.ContentType = "application/json";
			response.Write(new JavaScriptSerializer().Serialize(this));
		}
		#endregion

		public IEnumerable<INotification> ValidationErrors
		{
			get;
			protected set;
		}
	}
}