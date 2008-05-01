using DnugLeipzig.Definitions.Extensions;

using Graffiti.Core;

namespace DnugLeipzig.Plugins
{
	public static class PluginHelper
	{
		/// <summary>
		/// Ensure plugin initialization occurs before querying the plugin settings.
		/// </summary>
		static void EnsureInitialized()
		{
			Events.Instance();
		}

		/// <summary>
		/// Determines whether the plugin is enabled.
		/// </summary>
		/// <typeparam name="TPlugin">The type of the plugin.</typeparam>
		/// <returns>
		/// 	<c>true</c> if the plugin is enabled; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsPluginEnabled<TPlugin>()
			where TPlugin : GraffitiEvent
		{
			EnsureInitialized();

			EventDetails eventDetails = Events.GetEvent(typeof(TPlugin).GetPluginName());
			
			return eventDetails.Enabled;
		}

		/// <summary>
		/// Returns the plugin with current settings.
		/// </summary>
		/// <typeparam name="TPlugin">The type of the plugin.</typeparam>
		/// <returns></returns>
		public static TPlugin GetPluginWithCurrentSettings<TPlugin>()
			where TPlugin : GraffitiEvent
		{
			EnsureInitialized();

			EventDetails eventDetails = Events.GetEvent(typeof(TPlugin).GetPluginName());
			return eventDetails.Event as TPlugin;
		}
	}
}