using System;
using System.Collections;

using Castle.Windsor;

namespace DnugLeipzig.Definitions
{
	public static class IoC
	{
		public static IWindsorContainer Container
		{
			get
			{
				IWindsorContainer result = GlobalContainer;
				if (result == null)
				{
					throw new InvalidOperationException(
						"The container has not been initialized! Please call IoC.Initialize(container) before using it.");
				}
				return result;
			}
		}

		public static bool IsInitialized
		{
			get { return GlobalContainer != null; }
		}

		static IWindsorContainer GlobalContainer
		{
			get;
			set;
		}

		public static void Initialize(IWindsorContainer windsorContainer)
		{
			GlobalContainer = windsorContainer;
		}

		/// <summary>
		/// Tries to resolve the component, but return null instead of throwing if it is not there. Useful for optional
		/// dependencies.
		/// </summary>
		public static T TryResolve<T>()
		{
			return TryResolve(default(T));
		}

		/// <summary>
		/// Tries to resolve the component, but return the default  value if could not find it, instead of throwing. Useful for
		/// optional dependencies.
		/// </summary>
		public static T TryResolve<T>(T defaultValue)
		{
			if (Container.Kernel.HasComponent(typeof(T)) == false)
			{
				return defaultValue;
			}
			return Container.Resolve<T>();
		}

		public static object Resolve(Type serviceType)
		{
			return Container.Resolve(serviceType);
		}

		public static T Resolve<T>()
		{
			return Container.Resolve<T>();
		}

		public static T Resolve<T>(string name)
		{
			return Container.Resolve<T>(name);
		}

		public static T Resolve<T>(object argumentsAsAnonymousType)
		{
			return Container.Resolve<T>(argumentsAsAnonymousType);
		}

		public static T Resolve<T>(IDictionary parameters)
		{
			return Container.Resolve<T>(parameters);
		}

		public static void Reset(IWindsorContainer containerToReset)
		{
			if (containerToReset == null)
			{
				return;
			}

			if (ReferenceEquals(GlobalContainer, containerToReset))
			{
				GlobalContainer = null;
			}
		}

		public static void Reset()
		{
			Reset(GlobalContainer);
		}

		public static T[] ResolveAll<T>()
		{
			return Container.ResolveAll<T>();
		}
	}
}