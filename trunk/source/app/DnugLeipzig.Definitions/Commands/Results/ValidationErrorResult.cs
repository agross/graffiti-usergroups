using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;

namespace DnugLeipzig.Definitions.Commands.Results
{
	public class ValidationErrorResult : IHttpResponse
	{
		public ValidationErrorResult(ICollection<string> validationErrors)
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

		public ICollection<string> ValidationErrors
		{
			get;
			protected set;
		}
	}
}