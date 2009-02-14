using Graffiti.Core;

namespace DnugLeipzig.Definitions.GraffitiIntegration
{
	public interface IGraffitiEmailContext
	{
		object Put(string key, object value);

		EmailTemplateToolboxContext ToEmailTemplateToolboxContext();
	}
}