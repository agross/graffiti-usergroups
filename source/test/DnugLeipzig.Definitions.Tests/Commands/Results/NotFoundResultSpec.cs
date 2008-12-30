using System.Web;

using DnugLeipzig.Definitions.Commands.Results;
using DnugLeipzig.ForTesting;
using DnugLeipzig.ForTesting.HttpMocks;

using MbUnit.Framework;

namespace DnugLeipzig.Definitions.Tests.Commands.Results
{
	public class When_a_resource_could_not_be_found : Spec
	{
		HttpSimulator _request;
		IHttpResponse _sut;

		protected override void Establish_context()
		{
			_sut = new NotFoundResult();
			_request = new HttpSimulator().SimulateRequest();
		}

		protected override void Cleanup_after()
		{
			_request.Dispose();
		}

		protected override void Because()
		{
			_sut.Render(HttpContext.Current.Response);
		}

		[Test]
		public void It_should_set_Http_404_status()
		{
			Assert.AreEqual(404, HttpContext.Current.Response.StatusCode);
		}
	}
}