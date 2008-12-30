using System.Web;

using DnugLeipzig.Definitions.Commands.Results;
using DnugLeipzig.ForTesting;
using DnugLeipzig.ForTesting.HttpMocks;

using MbUnit.Framework;

namespace DnugLeipzig.Definitions.Tests.Commands.Results
{
	public class When_an_error_occurs : Spec
	{
		HttpSimulator _request;
		IHttpResponse _sut;

		protected override void Establish_context()
		{
			_sut = new ErrorResult();
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
		public void It_should_set_Http_500_status()
		{
			Assert.AreEqual(500, HttpContext.Current.Response.StatusCode);
		}
	}
}