using System.Web;

using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Definitions.Commands.Results;
using DnugLeipzig.ForTesting;
using DnugLeipzig.ForTesting.HttpMocks;

using MbUnit.Framework;

namespace DnugLeipzig.Definitions.Tests.Commands.Results
{
	public class ValidationErrorResultSpec_Empty : Spec
	{
		ICommandResult _sut;

		protected override void Before_each_spec()
		{
			_sut = new ValidationErrorResult(null);
		}

		[Test]
		public void It_should_render_the_validation_error_messages()
		{
			using (HttpSimulator request = new HttpSimulator().SimulateRequest())
			{
				_sut.Render(HttpContext.Current.Response);

				Assert.AreEqual("{\"ValidationErrors\":null}", request.ResponseText);
			}
		}
	}
}