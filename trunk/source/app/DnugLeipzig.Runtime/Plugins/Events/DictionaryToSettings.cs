using System.Collections.Specialized;
using System.Web;

using DnugLeipzig.Definitions.Extensions;
using DnugLeipzig.Runtime.Mapping;

namespace DnugLeipzig.Runtime.Plugins.Events
{
	public class DictionaryToSettings : TypeMapper<NameValueCollection, Settings>
	{
		public DictionaryToSettings()
		{
			From(x => x[EventPlugin.Fields.CreateTargetCategoryAndFields].IsSelected())
				.To(x => x.CreateTargetCategoryAndFields);
			From(x => x[EventPlugin.Fields.MigrateFieldValues].IsSelected())
				.To(x => x.MigrateFieldValues);

			From(x => HttpUtility.HtmlEncode(x[EventPlugin.Fields.CategoryName]))
				.To(x => x.CategoryName);
			From(x => x[EventPlugin.Fields.StartDate])
				.To(x => x.StartDate);
			From(x => x[EventPlugin.Fields.EndDate])
				.To(x => x.EndDate);
			From(x => x[EventPlugin.Fields.Speaker])
				.To(x => x.Speaker);
			From(x => x[EventPlugin.Fields.YearQueryString])
				.To(x => x.YearQueryString);
			From(x => x[EventPlugin.Fields.DateFormat])
				.To(x => x.DateFormat);
			From(x => x[EventPlugin.Fields.ShortEndDateFormat])
				.To(x => x.ShortEndDateFormat);
			From(x => x[EventPlugin.Fields.Location])
				.To(x => x.Location);
			From(x => x[EventPlugin.Fields.UnknownText])
				.To(x => x.UnknownText);
			From(x => x[EventPlugin.Fields.LocationUnknown])
				.To(x => x.LocationUnknown);
			From(x => x[EventPlugin.Fields.DefaultLocation])
				.To(x => x.DefaultLocation);
			From(x => x[EventPlugin.Fields.RegistrationNeeded])
				.To(x => x.RegistrationNeeded);
			From(x => x[EventPlugin.Fields.RegistrationRecipient])
				.To(x => x.RegistrationRecipient);
			From(x => x[EventPlugin.Fields.DefaultRegistrationRecipient])
				.To(x => x.DefaultRegistrationRecipient);
			From(x => x[EventPlugin.Fields.MaximumNumberOfRegistrations])
				.To(x => x.MaximumNumberOfRegistrations);
			From(x => x[EventPlugin.Fields.DefaultMaximumNumberOfRegistrations])
				.To(x => x.DefaultMaximumNumberOfRegistrations);
			From(x => x[EventPlugin.Fields.RegistrationList])
				.To(x => x.RegistrationList);
			From(x => x[EventPlugin.Fields.RegistrationMailSubject])
				.To(x => x.RegistrationMailSubject);
		}
	}
}