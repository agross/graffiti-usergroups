using System;
using System.Collections;
using System.Web;

using DnugLeipzig.Definitions.Extensions;

using Graffiti.Core;

namespace DnugLeipzig.DemoSite.Macros
{
	[Chalk("demoSite")]
	public class DemoSiteMacros
	{
		public bool SetupComplete()
		{
			return false;
		}

		public string RegisterButton(IDictionary properties)
		{
			string scriptPath =
				VirtualPathUtility.ToAbsolute(String.Format("~/files/themes/{0}/demosite/DemoSite.ashx",
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
	}
}