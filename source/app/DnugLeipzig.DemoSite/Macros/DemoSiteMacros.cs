using System;
using System.Collections;
using System.Web;

using DnugLeipzig.Definitions.Extensions;
using DnugLeipzig.Runtime.Plugins;

using Graffiti.Core;

namespace DnugLeipzig.DemoSite.Macros
{
	[Chalk("demoSite")]
	public class DemoSiteMacros
	{
		public bool SetupComplete()
		{
			EventDetails eventEventDetails = Events.GetEvent(typeof(EventPlugin).GetPluginName());
			EventDetails talkEventDetails = Events.GetEvent(typeof(TalkPlugin).GetPluginName());
			
			return eventEventDetails.Enabled && talkEventDetails.Enabled;
		}

		public string SetupButton(IDictionary properties)
		{
			string scriptPath =
				VirtualPathUtility.ToAbsolute(String.Format("~/files/themes/{0}/demosite/handlers/DemoSite.ashx",
				                                            GraffitiContext.Current.Theme));

			string cssClass = properties.GetAsAttribute("class");
			string text = properties.GetAsAttribute("value");
			string id = properties.GetAsAttribute("id");

			return string.Format("<input {0} {1} {2} type=\"button\" onclick=\"DemoSite.setupDemoSite('{3}');\" />",
			                     id,
			                     cssClass,
			                     text,
			                     scriptPath);
		}

		public string EventCategoryName()
		{
			EventPlugin eventPlugin = PluginHelper.GetPluginWithCurrentSettings<EventPlugin>();
			return eventPlugin.CategoryName;
		}

		public string TalkCategoryName()
		{
			TalkPlugin talkPlugin = PluginHelper.GetPluginWithCurrentSettings<TalkPlugin>();
			return talkPlugin.CategoryName;
		}
	}
}