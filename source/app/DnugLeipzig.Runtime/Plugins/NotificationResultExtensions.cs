using DnugLeipzig.Definitions.Mapping;
using DnugLeipzig.Definitions.Validation;

namespace DnugLeipzig.Runtime.Plugins
{
	public static class NotificationResultExtensions
	{
		static readonly IMapper<ValidationReport, PluginSettingsResult> Mapper = new ValidationResultToPluginSettingsResult();

		public static PluginSettingsResult Interpret(this ValidationReport validationReport)
		{
			PluginSettingsResult result = new PluginSettingsResult();
			Mapper.Map(validationReport, result);

			return result;
		}
	}
}