using DnugLeipzig.Definitions.Validation;
using DnugLeipzig.Runtime.Validation;

namespace DnugLeipzig.Runtime.Plugins.Talks.Validation
{
	public static class TalkErrors
	{
		// TODO: English
		public static INotification InvalidDate(string field)
		{
			return new ValidationError("Please enter a valid date.", field);
		}
	}
}