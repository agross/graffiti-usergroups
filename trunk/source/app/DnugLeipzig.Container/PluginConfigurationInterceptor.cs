using System;
using System.Reflection;

using Castle.Core.Interceptor;

using DnugLeipzig.Definitions.Extensions;

using Graffiti.Core;

namespace DnugLeipzig.Container
{
	internal class PluginConfigurationInterceptor : IInterceptor
	{
		static void EnsureInitialized()
		{
			Events.Instance();
		}

		static EventDetails EnsureInitializedAndEnabled(Type pluginType)
		{
			EnsureInitialized();

			EventDetails eventDetails = Events.GetEvent(pluginType.GetPluginName());

			if (!eventDetails.Enabled)
			{
				throw new InvalidOperationException(String.Format("The plug-in '{0}' is not enabled.", pluginType.Name));
			}

			return eventDetails;
		}

		static object InvokeOnCurrentInstance(IInvocation invocation)
		{
			Type pluginType = invocation.TargetType;

			EventDetails eventDetails = EnsureInitializedAndEnabled(pluginType);
			MethodInfo method = invocation.GetConcreteMethodInvocationTarget();

			return method.Invoke(eventDetails.Event, invocation.Arguments);
		}

		#region Implementation of IInterceptor
		public void Intercept(IInvocation invocation)
		{
			invocation.ReturnValue = InvokeOnCurrentInstance(invocation);
		}
		#endregion
	}
}