using System;
using System.Linq;
using System.Collections.Generic;

using Graffiti.Core;

using MbUnit.Framework;

namespace DnugLeipzig.Extensions.Tests
{
	[TestFixture]
	public class EventSortingTests
	{
		const int PostSetsToGenerate = 4;
		const string StartDateField = "StartDate";
		PostCollection _posts;
		PostCollection _originalPosts;

		[SetUp]
		public void SetUp()
		{
			_posts = new PostCollection();
			_originalPosts = new PostCollection();

			for (int i = 0; i < PostSetsToGenerate; i++)
			{
				// Create set of four posts on a given date (CreatedOn), two with the same event data, two without event dates at all.
				_posts.Add(CreatePost(String.Format("A (Has date) {0}", i),
				                      DateTime.Now.Date.Add(TimeSpan.FromDays(i)),
				                      DateTime.Now.Date.Add(TimeSpan.FromDays(i))));
				_posts.Add(CreatePost(String.Format("B (Has date) {0}", i),
				                      DateTime.Now.Date.Add(TimeSpan.FromDays(i)),
				                      DateTime.Now.Date.Add(TimeSpan.FromDays(i))));
				_posts.Add(CreatePost(String.Format("A (No date) {0}", i), DateTime.Now.Date.Add(TimeSpan.FromDays(i)), null));
				_posts.Add(CreatePost(String.Format("B (No date) {0}", i), DateTime.Now.Date.Add(TimeSpan.FromDays(i)), null));
			}

			_originalPosts.AddRange(_posts);
		}

		static Post CreatePost(string title, DateTime createdOn, DateTime? eventDate)
		{
			var post = new Post();
			post.Title = title;

			// "Veranstaltungen" in test database.
			post.CategoryId = 2;

			post.CreatedOn = createdOn;

			if (eventDate.HasValue)
			{
				post.CustomFields().Add(StartDateField, eventDate.Value.ToString("d"));
			}

			return post;
		}

		[Test]
		public void SortsEventIndexDescending()
		{
			List<Post> sorted = _posts.OrderBy(p => p, new DateDescendingPostComparer(StartDateField)).ToList();

			CollectionAssert.AreCountEqual(_originalPosts, sorted);
			CollectionAssert.AreEquivalent(_originalPosts, sorted);

			// Assert that
			// - posts without date come first,
			// - posts following the first date post also have dates,
			// - posts without date are sorted by title (ascending),
			// - posts with date are sorted by date (descending).
			bool firstDatePostReached = false;
			DateTime? lastDate = null;
			Post lastPost = null;
			foreach (Post post in sorted)
			{
				DateTime date;
				bool hasDate = DateTime.TryParse(post[StartDateField], out date);

				if (hasDate)
				{
					firstDatePostReached = true;
				}

				if (firstDatePostReached && !hasDate)
				{
					Assert.Fail("There are posts without dates following a post with a date.");
				}

				if (!firstDatePostReached && lastPost != null)
				{
					Assert.GreaterEqualThan(post.Title, lastPost.Title);
				}

				if (firstDatePostReached && lastDate != null && hasDate)
				{
					Assert.LowerEqualThan(date, lastDate.Value, post.Title+ " " +lastPost.Title);
				}

				lastPost = post;
				lastDate = hasDate ? date : (DateTime?) null;
			}
		}
		
		[Test]
		public void SortsEventIndexAscending()
		{
			List<Post> sorted = _posts.OrderBy(p => p, new DateAscendingPostComparer(StartDateField)).ToList();

			CollectionAssert.AreCountEqual(_originalPosts, sorted);
			CollectionAssert.AreEquivalent(_originalPosts, sorted);

			// Assert that
			// - posts without date come first,
			// - posts following the first date post also have dates,
			// - posts without date are sorted by title (ascending),
			// - posts with date are sorted by date (ascending).
			bool firstDatePostReached = false;
			DateTime? lastDate = null;
			Post lastPost = null;
			foreach (Post post in sorted)
			{
				DateTime date;
				bool hasDate = DateTime.TryParse(post[StartDateField], out date);

				if (hasDate)
				{
					firstDatePostReached = true;
				}

				if (firstDatePostReached && !hasDate)
				{
					Assert.Fail("There are posts without dates following a post with a date.");
				}

				if (!firstDatePostReached && lastPost != null)
				{
					Assert.GreaterEqualThan(post.Title, lastPost.Title);
				}

				if (firstDatePostReached && lastDate != null && hasDate)
				{
					Assert.GreaterEqualThan(date, lastDate.Value, post.Title + " " + lastPost.Title);
				}

				lastPost = post;
				lastDate = hasDate ? date : (DateTime?) null;
			}
		}
	}
}