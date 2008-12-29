using Graffiti.Core;

namespace DnugLeipzig.Definitions.Configuration
{
	public interface IGraffitiSettings
	{
		CommentSettings Comments
		{
			get;
		}

		GraffitiSiteSettings Site
		{
			get;
		}
	}
}