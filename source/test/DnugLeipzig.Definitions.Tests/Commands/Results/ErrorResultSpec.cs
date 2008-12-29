using System.Web;

using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Definitions.Commands.Results;
using DnugLeipzig.ForTesting;
using DnugLeipzig.ForTesting.HttpMocks;

using MbUnit.Framework;

namespace DnugLeipzig.Definitions.Tests.Commands.Results
{
	public class ErrorResultSpec : Spec
	{
		ICommandResult _sut;

		protected override void Before_each_spec()
		{
			_sut = new ErrorResult();
		}

		[Test]
		public void It_should_render_HTTP_500()
		{
			using (new HttpSimulator().SimulateRequest())
			{
				_sut.Render(HttpContext.Current.Response);

				Assert.AreEqual(500, HttpContext.Current.Response.StatusCode);
			}
		}
	}
}