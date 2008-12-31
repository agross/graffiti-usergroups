using Graffiti.Core;

namespace DnugLeipzig.Definitions.Configuration
{
	public interface IGraffitiEmailContext
	{
		object Put(string key, object value);

		EmailTemplateToolboxContext ToEmailTemplateToolboxContext();
	}
}