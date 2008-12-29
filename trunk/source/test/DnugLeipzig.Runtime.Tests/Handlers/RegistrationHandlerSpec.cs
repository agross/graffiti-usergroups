using System;
using System.Web;

using DnugLeipzig.Definitions;
using DnugLeipzig.ForTesting;
using DnugLeipzig.ForTesting.HttpMocks;
using DnugLeipzig.Runtime.Handlers;

using MbUnit.Framework;

using Rhino.Testing.AutoMocking;

namespace DnugLeipzig.Runtime.Tests.Handlers
{
	public class RegistrationHandlerSpec : Spec
	{
		AutoMockingContainer _container;
		RegistrationHandler _sut;

		protected override void Before_each_spec()
		{
			_sut = new RegistrationHandler();

			_container = new AutoMockingContainer(Mocks);
			_container.Initialize();
			IoC.Initialize(_container);
		}

		protected override void After_each_spec()
		{
			IoC.Container.Dispose();
			IoC.Reset();
		}

		[Test]
		public void ShouldReturn403ForbiddenIfNoUsingHttpPost()
		{
			using (new HttpSimulator().SimulateRequest(new Uri("http://foo"), HttpVerb.GET))
			{
				_sut.ProcessRequest(HttpContext.Current);

				Assert.AreEqual(403, HttpContext.Current.Response.StatusCode);
			}
		}

		//		[Test]
		//		public void ShouldReturn500InternalServerErrorForUnknownCommands()
		//		{
		//			using (new HttpSimulator().SimulateRequest(new Uri(String.Format("http://foo?command={0}",
		//			                                                                 Guid.NewGuid())),
		//			                                           HttpVerb.POST))
		//			{
		//				_sut.ProcessRequest(HttpContext.Current);
		//
		//				Assert.AreEqual(500, HttpContext.Current.Response.StatusCode);
		//			}
		//		}

//		[RowTest]
//		[Row("register")]
//		public void ShouldReturn200OKForKnownCommands(string command)
//		{
//			using (new HttpSimulator().SimulateRequest(new Uri(String.Format("http://foo?command={0}", command)),
//			                                           HttpVerb.POST))
//			{
//				_sut.ProcessRequest(HttpContext.Current);
//
//				Assert.AreEqual(200, HttpContext.Current.Response.StatusCode);
//			}
//		}
	}
}