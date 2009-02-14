using System;
using System.Linq;

using DnugLeipzig.Definitions.Validation;
using DnugLeipzig.Runtime.Mapping;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Plugins
{
	public class ValidationReportToExceptionTemplate : TypeMapper<ValidationReport, ExceptionTemplate>
	{
		public ValidationReportToExceptionTemplate()
		{
			From(x =>
				{
					if (x.HasErrors)
					{
						return StatusType.Error;
					}

					if (x.HasWarnings)
					{
						return StatusType.Warning;
					}

					return StatusType.Success;
				})
				.To(x => x.StatusType);

			From(x =>
				{
					if (x.HasErrors)
					{
						return x.Errors.First().Message;
					}

					if (x.HasWarnings)
					{
						return x.Warnings.First().Message;
					}

					return String.Empty;
				})
				.To(x => x.Message);
			
			From(x =>
				{
					if (x.HasErrors)
					{
						return x.Errors.First().AffectedFormFields;
					}

					if (x.HasWarnings)
					{
						return x.Warnings.First().AffectedFormFields;
					}

					return null;
				})
				.To(x => x.AffectedFormFields);
		}
	}
}