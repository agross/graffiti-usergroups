using System;
using System.Diagnostics;

using DnugLeipzig.Definitions.Extensions;
using DnugLeipzig.Plugins;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Configuration
{
	public abstract class PluginConfiguration<TPlugin> where TPlugin : GraffitiEvent
	{
		protected TPlugin PluginInstance;

		protected void EnsureCurrentInstance()
		{
			Debug.WriteLine(String.Format("{0}.EnsureCurrentInstance", GetType().Name));

			if (!PluginHelper.IsPluginEnabled<TPlugin>())
			{
				throw new InvalidOperationException(String.Format("The plugin '{0}' has not been enabled.", typeof(TPlugin).Name));
			}

			PluginInstance = PluginHelper.GetPluginWithCurrentSettings<TPlugin>();

			EventDetails eventDetails = Events.GetEvent(typeof(TPlugin).GetPluginName());
			PluginInstance = eventDetails.Event as TPlugin;
		}
	}
}