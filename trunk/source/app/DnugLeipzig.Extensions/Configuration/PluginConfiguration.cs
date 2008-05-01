using System;
using System.Diagnostics;

using DnugLeipzig.Definitions.Extensions;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Configuration
{
	public abstract class PluginConfiguration<TPlugin> where TPlugin : GraffitiEvent
	{
		protected TPlugin PluginInstance;

		//protected abstract string CacheKey
		//{
		//    get;
		//}

		protected void EnsureCurrentInstance()
		{
			Debug.WriteLine(String.Format("{0}.EnsureCurrentInstance", GetType().Name));

			// We do not use the HTTP cache.
			//PluginInstance = HttpContext.Current.Cache.Get(CacheKey) as TPlugin;
			//if (PluginInstance != null)
			//{
			//    Debug.WriteLine("--> Cached");
			//    return;
			//}

			//Debug.WriteLine("--> Not cached");

			// Ensure plugin initialization occurs before we query the plugin settings.
			Events.Instance();

			EventDetails eventDetails = Events.GetEvent(typeof(TPlugin).GetPluginName());
			if (!eventDetails.Enabled)
			{
				throw new InvalidOperationException(String.Format("The plugin '{0}' has not been enabled.", eventDetails.Event.Name));
			}

			PluginInstance = eventDetails.Event as TPlugin;

			//HttpContext.Current.Cache.Add(CacheKey,
			//                              PluginInstance,
			//                              null,
			//                              Cache.NoAbsoluteExpiration,
			//                              Cache.NoSlidingExpiration,
			//                              CacheItemPriority.NotRemovable,
			//                              null);
		}
	}
}