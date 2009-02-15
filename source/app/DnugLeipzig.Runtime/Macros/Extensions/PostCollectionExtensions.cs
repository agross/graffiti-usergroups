using System;
using System.Collections.Generic;
using System.Linq;

using DnugLeipzig.Definitions.Services;
using DnugLeipzig.Runtime.Macros.PostComparer;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Macros.Extensions
{
	internal static class PostCollectionExtensions
	{
		public static IEnumerable<Post> HasDate(this IEnumerable<Post> posts, string dateField)
		{
			return posts.Where(post => PostExtensions.HasDate(post, dateField));
		}

		public static IEnumerable<Post> IsInPastYear(this IEnumerable<Post> posts, string dateField)
		{
			return posts.Where(post => post.IsInPastYear(dateField));
		}

		public static IEnumerable<Post> IsInPast(this IEnumerable<Post> posts, string dateField)
		{
			return posts.Where(post => post.IsInPast(dateField));
		}

		public static IEnumerable<Post> IsInFuture(this IEnumerable<Post> posts, string dateField)
		{
			return posts.Where(post => post.IsInFuture(dateField));
		}

		public static IEnumerable<Post> IsInYear(this IEnumerable<Post> posts, string dateField, DateTime year)
		{
			return posts.Where(post => post.IsInYear(dateField, year));
		}

		public static IEnumerable<Post> SortAscending(this IEnumerable<Post> posts, string dateField)
		{
			return posts.OrderBy(p => p, new DateAscendingPostComparer(dateField));
		}

		public static IEnumerable<Post> SortDescending(this IEnumerable<Post> posts, string dateField)
		{
			return posts.OrderBy(p => p, new DateDescendingPostComparer(dateField));
		}

		public static IEnumerable<Post> LimitTo(this IEnumerable<Post> posts, int maximumNumberOfPosts)
		{
			return posts.Take(maximumNumberOfPosts);
		}

		public static IEnumerable<Post> RegistrationNeeded(this IEnumerable<Post> posts, string registrationNeededField)
		{
			return posts.Where(post => post.RegistrationNeeded(registrationNeededField));
		}

		public static IEnumerable<Post> RegistrationPossible(this IEnumerable<Post> posts,
		                                                     string numberOfRegistrationsField,
		                                                     string maximumNumberOfRegistrationsField,
		                                                     string earliestRegistrationField,
		                                                     string latestRegistrationField,
		                                                     string startDateField,
		                                                     IClock clock)
		{
			return posts.Where(post => post.RegistrationPossible(numberOfRegistrationsField,
			                                                     maximumNumberOfRegistrationsField,
			                                                     earliestRegistrationField,
			                                                     latestRegistrationField,
			                                                     startDateField,
			                                                     clock));
		}
	}
}