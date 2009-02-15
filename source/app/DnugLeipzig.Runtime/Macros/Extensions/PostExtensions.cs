using System;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Extensions;
using DnugLeipzig.Definitions.Services;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Macros.Extensions
{
	internal static class PostExtensions
	{
		static readonly IClock Clock = IoC.Resolve<IClock>();

		public static bool HasDate(this Post post, string dateField)
		{
			return post[dateField].IsDate();
		}

		public static bool IsInPastYear(this Post post, string dateField)
		{
			return post[dateField].AsEventDate().Year < DateTime.Now.Year;
		}

		public static bool IsInPast(this Post post, string dateField)
		{
			return post[dateField].AsEventDate().Date < DateTime.Now.Date;
		}

		public static bool IsInFuture(this Post post, string dateField)
		{
			return post[dateField].AsEventDate().Date >= DateTime.Now.Date;
		}

		public static bool IsInYear(this Post post, string dateField, DateTime year)
		{
			return post[dateField].AsEventDate().Year == year.Year;
		}

		public static bool RegistrationNeeded(this Post post, string registrationNeededField)
		{
			return post[registrationNeededField].IsSelected();
		}

		public static bool RegistrationPossible(this Post post,
		                                        string numberOfRegistrationsField,
		                                        string maximumNumberOfRegistrationsField,
		                                        string earliestRegistrationField,
		                                        string latestRegistrationField,
		                                        string startDate,
		                                        IClock clock)
		{
			// Registration is always possible, but attendees will be notified if they are on the waiting list.
			//return post[numberOfRegistrationsField].ToInt(0) <
			//       post[maximumNumberOfRegistrationsField].ToInt(int.MaxValue);

			if (!post[earliestRegistrationField].HasValue() || !post[latestRegistrationField].HasValue())
			{
				return true;
			}

			DateTime earliest = post[earliestRegistrationField].ToDate(DateTime.MinValue);
			DateTime latest = post[latestRegistrationField].ToDate(DateTime.MaxValue);
			DateTime eventStart = post[startDate].AsEventDate();
			if (latest > eventStart)
			{
				latest = eventStart;
			}

			return Clock.Now.IsInRange(earliest, latest);
		}
	}
}