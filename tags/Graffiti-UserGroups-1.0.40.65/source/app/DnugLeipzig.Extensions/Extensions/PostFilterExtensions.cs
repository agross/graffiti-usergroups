using System;
using System.Collections.Generic;
using System.Linq;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Extensions
{
	internal static class PostFilterExtensions
	{
		#region Date/Time Related
		public static IEnumerable<Post> HasDate(this IEnumerable<Post> posts, string dateFieldName)
		{
			return posts.Where(post => post.HasDate(dateFieldName));
		}

		public static IEnumerable<Post> IsInPastYear(this IEnumerable<Post> posts, string dateFieldName)
		{
			return posts.Where(post => post.IsInPastYear(dateFieldName));
		}

		public static IEnumerable<Post> IsInPast(this IEnumerable<Post> posts, string dateFieldName)
		{
			return posts.Where(post => post.IsInPast(dateFieldName));
		}

		public static IEnumerable<Post> IsInFuture(this IEnumerable<Post> posts, string dateFieldName)
		{
			return posts.Where(post => post.IsInFuture(dateFieldName));
		}

		public static IEnumerable<Post> IsInYear(this IEnumerable<Post> posts, string dateFieldName, DateTime year)
		{
			return posts.Where(post => post.IsInYear(dateFieldName, year));
		}
		#endregion

		#region Sorting
		public static IEnumerable<Post> SortAscending(this IEnumerable<Post> posts, string dateFieldName)
		{
			return posts.OrderBy(p => p, new DateAscendingPostComparer(dateFieldName));
		}

		public static IEnumerable<Post> SortDescending(this IEnumerable<Post> posts, string dateFieldName)
		{
			return posts.OrderBy(p => p, new DateDescendingPostComparer(dateFieldName));
		}
		#endregion

		public static IEnumerable<Post> LimitTo(this IEnumerable<Post> posts, int maximumNumberOfPosts)
		{
			return posts.Take(maximumNumberOfPosts);
		}

		public static IEnumerable<Post> RegistrationNeeded(this IEnumerable<Post> posts, string registrationNeededFieldName)
		{
			return posts.Where(post => post.RegistrationNeeded(registrationNeededFieldName));
		}

		public static IEnumerable<Post> RegistrationPossible(this IEnumerable<Post> posts,
		                                                     string numberOfRegistrationsFieldName,
		                                                     string maximumNumberOfRegistrationsFieldName)
		{
			return
				posts.Where(post => post.RegistrationPossible(numberOfRegistrationsFieldName, maximumNumberOfRegistrationsFieldName));
		}
	}
}