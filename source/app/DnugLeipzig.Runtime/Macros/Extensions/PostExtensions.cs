using System;

using DnugLeipzig.Definitions.Extensions;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Extensions
{
	internal static class PostExtensions
	{
		#region Date/Time Related
		public static bool HasDate(this Post post, string dateFieldName)
		{
			return post[dateFieldName].IsDate();
		}

		public static bool IsInPastYear(this Post post, string dateFieldName)
		{
			return post[dateFieldName].AsEventDate().Year < DateTime.Now.Year;
		}

		public static bool IsInPast(this Post post, string dateFieldName)
		{
			return post[dateFieldName].AsEventDate().Date < DateTime.Now.Date;
		}

		public static bool IsInFuture(this Post post, string dateFieldName)
		{
			return post[dateFieldName].AsEventDate().Date >= DateTime.Now.Date;
		}

		public static bool IsInYear(this Post post, string dateFieldName, DateTime year)
		{
			return post[dateFieldName].AsEventDate().Year == year.Year;
		}
		#endregion

		public static bool RegistrationNeeded(this Post post, string registrationNeededFieldName)
		{
			return post[registrationNeededFieldName].IsSelected();
		}

		public static bool RegistrationPossible(this Post post,
		                                        string numberOfRegistrationsFieldName,
		                                        string maximumNumberOfRegistrationsFieldName)
		{
			// Registration is always possible, but attendees will be notified if they are on the waiting list.
			return true;

			//return post[numberOfRegistrationsFieldName].ToInt(0) <
			//       post[maximumNumberOfRegistrationsFieldName].ToInt(int.MaxValue);
		}
	}
}