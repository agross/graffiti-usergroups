using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Mapping;
using DnugLeipzig.Definitions.Validation;

namespace DnugLeipzig.Runtime.Plugins
{
	public static class ValidationReportExtensions
	{
		static readonly IMapper<ValidationReport, ExceptionTemplate> Mapper = new ValidationReportToExceptionTemplate();

		public static ExceptionTemplate Interpret(this ValidationReport validationReport)
		{
			ExceptionTemplate result = new ExceptionTemplate();
			Mapper.Map(validationReport, result);

			return result;
		}
	}
}