using DnugLeipzig.Definitions.Extensions;

using Graffiti.Core;

namespace DnugLeipzig.Plugins
{
	public static class PluginHelper
	{
		/// <summary>
		/// Ensure plugiin initialization occurs before querying the plugiin settings.
		/// </summary>
		static void EnsureInitialized()
		{
			Events.Instance();
		}

		/// <summary>
		/// Determines whether the plug-in is enabled.
		/// </summary>
		/// <typeparam name="TPlugin">The type of the plug-in.</typeparam>
		/// <returns>
		/// 	<c>true</c> if the plug-in is enabled; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsPluginEnabled<TPlugin>()
			where TPlugin : GraffitiEvent
		{
			EnsureInitialized();

			EventDetails eventDetails = Events.GetEvent(typeof(TPlugin).GetPluginName());
			
			return eventDetails.Enabled;
		}

		/// <summary>
		/// Returns the plug-in with current settings.
		/// </summary>
		/// <typeparam name="TPlugin">The type of the plug-in.</typeparam>
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