using System.Collections.Generic;
using System.Linq;

namespace DnugLeipzig.Definitions.Validation
{
	public class ValidationReport : List<INotification>
	{
		public bool HasErrors
		{
			get { return Exists(x => x.Severity == Severity.Error); }
		}

		public bool HasWarnings
		{
			get { return Exists(x => x.Severity == Severity.Warning); }
		}

		public IEnumerable<INotification> Errors
		{
			get { return this.Where(x => x.Severity == Severity.Error); }
		}

		public IEnumerable<INotification> Warnings
		{
			get { return this.Where(x => x.Severity == Severity.Warning); }
		}
	}
}