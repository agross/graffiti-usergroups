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
			return post.Custom(dateFieldName).IsDate();
		}

		public static bool IsInPastYear(this Post post, string dateFieldName)
		{
			return post.Custom(dateFieldName).AsEventDate().Year < DateTime.Now.Year;
		}

		public static bool IsInPast(this Post post, string dateFieldName)
		{
			return post.Custom(dateFieldName).AsEventDate().Date < DateTime.Now.Date;
		}

		public static bool IsInFuture(this Post post, string dateFieldName)
		{
			return post.Custom(dateFieldName).AsEventDate().Date >= DateTime.Now.Date;
		}

		public static bool IsInYear(this Post post, string dateFieldName, DateTime year)
		{
			return post.Custom(dateFieldName).AsEventDate().Year == year.Year;
		}
		#endregion

		public static bool RegistrationNeeded(this Post post, string registrationNeededFieldName)
		{
			return post.Custom(registrationNeededFieldName).IsChecked();
		}

		public static bool RegistrationPossible(this Post post,
		                                        string numberOfRegistrationsFieldName,
		                                        string maximumNumberOfRegistrationsFieldName)
		{
			return post.Custom(numberOfRegistrationsFieldName).ToInt(0) <
			       post.Custom(maximumNumberOfRegistrationsFieldName).ToInt(int.MaxValue);
		}
	}
}