using System.Collections.Generic;
using System.Web;

using DnugLeipzig.Definitions.Commands.Results;
using DnugLeipzig.Definitions.Validation;
using DnugLeipzig.ForTesting;
using DnugLeipzig.ForTesting.HttpMocks;
using DnugLeipzig.Runtime.Validation;

using MbUnit.Framework;

namespace DnugLeipzig.Definitions.Tests.Commands.Results
{
	public class When_a_validation_error_result_is_rendered : Spec
	{
		HttpSimulator _request;
		IHttpResponse _sut;

		protected override void Establish_context()
		{
			_sut =
				new ValidationErrorResult(new List<INotification> { new ValidationError("foo"), new ValidationError("bar") });
			_request = new HttpSimulator().SimulateRequest();
		}

		protected override void Cleanup_after()
		{
			_request.Dispose();
		}

		protected override void Because()
		{
			_sut.Render(HttpContext.Current.Response);
			HttpContext.Current.Response.End();
		}

		[Test]
		public void It_should_set_Http_200_status()
		{
			Assert.AreEqual(200, HttpContext.Current.Response.StatusCode);
		}

		[Test]
		public void It_should_render_the_Json_content_type()
		{
			Assert.AreEqual("application/json", HttpContext.Current.Response.ContentType);
		}

		[Test]
		public void It_should_render_the_validation_error_messages()
		{
			StringAssert.Contains(_request.ResponseText, "foo");
			StringAssert.Contains(_request.ResponseText, "bar");
		}
	}

	public class When_an_empty_validation_error_result_is_rendered : Spec
	{
		HttpSimulator _request;
		IHttpResponse _sut;

		protected override void Establish_context()
		{
			_sut = new ValidationErrorResult(null);
			_request = new HttpSimulator().SimulateRequest();
		}

		protected override void Cleanup_after()
		{
			_request.Dispose();
		}

		protected override void Because()
		{
			_sut.Render(HttpContext.Current.Response);
			HttpContext.Current.Response.End();
		}

		[Test]
		public void It_should_render_an_empty_validation_error_message()
		{
			Assert.AreEqual("{\"ValidationErrors\":null}", _request.ResponseText);
		}
	}
}