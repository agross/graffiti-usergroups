using DnugLeipzig.Definitions.Configuration;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Configuration
{
	public class GraffitiCommentSettings : IGraffitiCommentSettings
	{
		public string Email
		{
			get { return CommentSettings.Get().Email; }
		}
	}
}