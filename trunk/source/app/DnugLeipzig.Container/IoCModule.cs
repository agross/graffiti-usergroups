using System.Runtime.CompilerServices;
using System.Web;

using Castle.Facilities.Logging;
using Castle.Windsor;

using DnugLeipzig.Definitions;
using DnugLeipzig.Runtime.Logging;

namespace DnugLeipzig.Container
{
	public class IoCModule : IHttpModule
	{
		IWindsorContainer _container;

		#region Implementation of IHttpModule
		/// <summary>
		/// Initializes a module and prepares it to handle requests.
		/// </summary>
		/// <param name="context">An <see cref="T:System.Web.HttpApplication"></see> that provides access to the methods,
		/// properties, and events common to all application objects within an ASP.NET application </param>
		public void Init(HttpApplication context)
		{
			if (!IoC.IsInitialized)
			{
				InitializeContainer(this);
			}
		}

		/// <summary>
		/// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule">
		/// </see>.
		/// </summary>
		public void Dispose()
		{
			if (_container != null)
			{
				// Release or cleanup managed resources.
				IoC.Reset(_container);
				_container.Dispose();
			}
		}
		#endregion

		[MethodImpl(MethodImplOptions.Synchronized)]
		void InitializeContainer(IoCModule self)
		{
			if (IoC.IsInitialized)
			{
				return;
			}
			_container = self.InitializeContainer();
		}

		IWindsorContainer InitializeContainer()
		{
			IoC.Initialize(CreateContainerWithMappings());
			return IoC.Container;
		}

		protected virtual IWindsorContainer CreateContainerWithMappings()
		{
			IWindsorContainer container = new WindsorContainer();

			container.AddFacility("LoggingFacility",
			                      new LoggingFacility(LoggerImplementation.Custom,
			                                          typeof(GraffitiLoggerFactory).AssemblyQualifiedName,
			                                          null));

			foreach (var registration in Registrations.Get())
			{
				container.Register(registration);
			}

			return container;
		}
	}
}