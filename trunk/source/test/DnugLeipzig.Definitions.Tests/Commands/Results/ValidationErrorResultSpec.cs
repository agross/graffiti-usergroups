using System.Collections.Generic;
using System.Web;

using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Definitions.Commands.Results;
using DnugLeipzig.ForTesting;
using DnugLeipzig.ForTesting.HttpMocks;

using MbUnit.Framework;

namespace DnugLeipzig.Definitions.Tests.Commands.Results
{
	public class ValidationErrorResultSpec : Spec
	{
		ICommandResult _sut;

		protected override void Before_each_spec()
		{
			_sut = new ValidationErrorResult(new List<string> { "foo", "bar" });
		}

		[Test]
		public void It_should_render_HTTP_200()
		{
			using (new HttpSimulator().SimulateRequest())
			{
				_sut.Render(HttpContext.Current.Response);

				Assert.AreEqual(200, HttpContext.Current.Response.StatusCode);
			}
		}

		[Test]
		public void It_should_render_the_JSON_content_type()
		{
			using (new HttpSimulator().SimulateRequest())
			{
				_sut.Render(HttpContext.Current.Response);

				Assert.AreEqual("application/json", HttpContext.Current.Response.ContentType);
			}
		}

		[Test]
		public void It_should_render_the_validation_error_messages()
		{
			using (HttpSimulator request = new HttpSimulator().SimulateRequest())
			{
				_sut.Render(HttpContext.Current.Response);

				StringAssert.Contains(request.ResponseText, "foo");
				StringAssert.Contains(request.ResponseText, "bar");
			}
		}
	}
}