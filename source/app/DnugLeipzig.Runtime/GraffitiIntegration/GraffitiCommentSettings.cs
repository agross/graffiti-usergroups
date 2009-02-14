using DnugLeipzig.Definitions.GraffitiIntegration;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.GraffitiIntegration
{
	public class GraffitiCommentSettings : IGraffitiCommentSettings
	{
		public string Email
		{
			get { return CommentSettings.Get().Email; }
		}
	}
}