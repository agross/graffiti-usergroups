using DnugLeipzig.Definitions.Extensions;
using DnugLeipzig.Definitions.Plugins.Talks;
using DnugLeipzig.Runtime.Plugins.Events.Validation;
using DnugLeipzig.Runtime.Validation;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Plugins.Talks.Validation
{
	public class TalkValidator : Validator<Post>, ITalkValidator
	{
		public TalkValidator(ITalkPluginConfigurationProvider config)
		{
			If(x => x[config.DateField].HasValue() && !x[config.DateField].IsDate())
				.AddNotification(EventErrors.InvalidDate(config.DateField));
		}
	}
}