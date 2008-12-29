using System.Web;

using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Definitions.Commands.Results;
using DnugLeipzig.ForTesting;
using DnugLeipzig.ForTesting.HttpMocks;

using MbUnit.Framework;

namespace DnugLeipzig.Definitions.Tests.Commands.Results
{
	public class When_a_resource_cannot_be_accessed : Spec
	{
		HttpSimulator _request;
		ICommandResult _sut;

		protected override void Establish_context()
		{
			_sut = new ForbiddenResult();
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
		public void It_should_set_Http_403_status()
		{
			Assert.AreEqual(403, HttpContext.Current.Response.StatusCode);
		}
	}
}