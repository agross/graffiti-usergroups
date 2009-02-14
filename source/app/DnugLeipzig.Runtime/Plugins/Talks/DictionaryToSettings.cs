using System.Collections.Specialized;
using System.Web;

using DnugLeipzig.Definitions.Extensions;
using DnugLeipzig.Runtime.Mapping;

namespace DnugLeipzig.Runtime.Plugins.Talks
{
	public class DictionaryToSettings : TypeMapper<NameValueCollection, Settings>
	{
		public DictionaryToSettings()
		{
			From(x => x[TalkPlugin.Fields.CreateTargetCategoryAndFields].IsSelected())
				.To(x => x.CreateTargetCategoryAndFields);
			From(x => x[TalkPlugin.Fields.MigrateFieldValues].IsSelected())
				.To(x => x.MigrateFieldValues);

			From(x => HttpUtility.HtmlEncode(x[TalkPlugin.Fields.CategoryName]))
				.To(x => x.CategoryName);
			From(x => x[TalkPlugin.Fields.Date])
				.To(x => x.Date);
			From(x => x[TalkPlugin.Fields.Speaker])
				.To(x => x.Speaker);
			From(x => x[TalkPlugin.Fields.YearQueryString])
				.To(x => x.YearQueryString);
		}
	}
}