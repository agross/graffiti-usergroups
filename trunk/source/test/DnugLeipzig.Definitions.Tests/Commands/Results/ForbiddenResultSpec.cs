using System.Web;

using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Definitions.Commands.Results;
using DnugLeipzig.ForTesting;
using DnugLeipzig.ForTesting.HttpMocks;

using MbUnit.Framework;

namespace DnugLeipzig.Definitions.Tests.Commands.Results
{
	public class ForbiddenResultSpec : Spec
	{
		ICommandResult _sut;

		protected override void Before_each_spec()
		{
			_sut = new ForbiddenResult();
		}

		[Test]
		public void It_should_render_HTTP_403()
		{
			using (new HttpSimulator().SimulateRequest())
			{
				_sut.Render(HttpContext.Current.Response);

				Assert.AreEqual(403, HttpContext.Current.Response.StatusCode);
			}
		}
	}
}