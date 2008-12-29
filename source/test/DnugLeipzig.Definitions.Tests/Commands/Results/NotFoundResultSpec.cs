using System.Web;

using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Definitions.Commands.Results;
using DnugLeipzig.ForTesting;
using DnugLeipzig.ForTesting.HttpMocks;

using MbUnit.Framework;

namespace DnugLeipzig.Definitions.Tests.Commands.Results
{
	public class NotFoundResultSpec : Spec
	{
		ICommandResult _sut;

		protected override void Before_each_spec()
		{
			_sut = new NotFoundResult();
		}

		[Test]
		public void It_should_render_HTTP_404()
		{
			using (new HttpSimulator().SimulateRequest())
			{
				_sut.Render(HttpContext.Current.Response);

				Assert.AreEqual(404, HttpContext.Current.Response.StatusCode);
			}
		}
	}
}