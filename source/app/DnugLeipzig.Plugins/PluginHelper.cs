using DnugLeipzig.Definitions.Configuration;
using DnugLeipzig.Definitions.Extensions;

using Graffiti.Core;

namespace DnugLeipzig.Plugins
{
	public static class PluginHelper
	{
		public static TPlugin GetPluginWithCurrentSettings<TPlugin>()
			where TPlugin : GraffitiEvent, ICategoryEnabledRepositoryConfiguration, new()
		{
			EventDetails eventDetails = Events.GetEvent(typeof(TPlugin).GetPluginName());
			return eventDetails.Event as TPlugin;
		}
	}
}