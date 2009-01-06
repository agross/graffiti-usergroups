using Graffiti.Core;

namespace DnugLeipzig.Runtime.Plugins
{
	public class PluginSettingsResult
	{
		public StatusType StatusType
		{
			get;
			set;
		}

		public string Message
		{
			get;
			set;
		}

		public bool Failed
		{
			get { return StatusType == StatusType.Error || StatusType == StatusType.Warning; }
		}
	}
}