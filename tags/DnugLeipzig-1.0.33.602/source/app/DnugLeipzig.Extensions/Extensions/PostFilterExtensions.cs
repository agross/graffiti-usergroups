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
			return from post in posts where post.Custom(dateFieldName).IsDate() select post;
		}

		public static IEnumerable<Post> IsInPastYear(this IEnumerable<Post> posts, string dateFieldName)
		{
			return from post in posts where post.Custom(dateFieldName).AsEventDate().Year < DateTime.Now.Year select post;
		}

		public static IEnumerable<Post> IsInPast(this IEnumerable<Post> posts, string dateFieldName)
		{
			return from post in posts where post.Custom(dateFieldName).AsEventDate().Date < DateTime.Now.Date select post;
		}

		public static IEnumerable<Post> IsInFuture(this IEnumerable<Post> posts, string dateFieldName)
		{
			return from post in posts where post.Custom(dateFieldName).AsEventDate().Date >= DateTime.Now.Date select post;
		}

		public static IEnumerable<Post> IsInYear(this IEnumerable<Post> posts, string dateFieldName, DateTime year)
		{
			return from post in posts where post.Custom(dateFieldName).AsEventDate().Year == year.Year select post;
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
	}
}