using System;
using System.Web;

using DnugLeipzig.Extensions.Configuration;
using DnugLeipzig.Extensions.Extensions;
using DnugLeipzig.Extensions.Repositories;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Macros
{
	public abstract class Macros
	{
		protected static readonly Graffiti.Core.Macros GraffitiMacros = new Graffiti.Core.Macros();
		readonly IConfigurationSource Configuration;
		protected IRepository<Post> Repository;

		protected Macros(IConfigurationSource configuration)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}

			Configuration = configuration;
		}

		public virtual string GetFeedUrl()
		{
			Category c = Repository.GetCategory();
			if (!String.IsNullOrEmpty(c.FeedUrlOverride))
			{
				return c.FeedUrlOverride;
			}

			return String.Format("{0}feed/", c.Url);
		}

		public virtual string GetCategoryLink()
		{
			Category c = Repository.GetCategory();
			return c.Url;
		}

		public virtual int? GetYearOfView()
		{
			return Util.GetYearOfView(Configuration.YearQueryString);
		}

		#region Date
		public bool HasDate(Post post)
		{
			return post.Custom(Configuration.SortRelevantDateField).IsDate();
		}

		public bool IsInCurrentYear(Post post)
		{
			return post.Custom(Configuration.SortRelevantDateField).AsEventDate().Year == DateTime.Now.Year;
		}

		public string Date(Post post)
		{
			return Date(post, null, null, null);
		}

		public string Date(Post post, string format)
		{
			return Date(post, format, null, null);
		}

		public string Date(Post post, string prefix, string suffix)
		{
			return Date(post, null, prefix, suffix);
		}

		public virtual string Date(Post post, string format, string prefix, string suffix)
		{
			if (!post.Custom(Configuration.SortRelevantDateField).IsDate())
			{
				return null;
			}

			string formattedDate;
			if (String.IsNullOrEmpty(format))
			{
				formattedDate = GraffitiMacros.FormattedDate(post.Custom(Configuration.SortRelevantDateField).AsEventDate());
			}
			else
			{
				formattedDate = post.Custom(Configuration.SortRelevantDateField).AsEventDate().ToString(format);
			}

			return HttpUtility.HtmlEncode(String.Format("{0}{1}{2}", prefix, formattedDate, suffix));
		}
		#endregion

		#region Speaker
		public virtual string Speaker(Post post)
		{
			return Speaker(post, null, null, null);
		}

		public virtual string Speaker(Post post, string prefix, string suffix)
		{
			return Speaker(post, null, prefix, suffix);
		}

		public virtual string Speaker(Post post, string defaultValue, string prefix, string suffix)
		{
			string speaker = post.Custom(Configuration.SpeakerField);
			if (String.IsNullOrEmpty(speaker))
			{
				speaker = defaultValue;

				if (String.IsNullOrEmpty(speaker))
				{
					return null;
				}
			}

			return HttpUtility.HtmlEncode(String.Format("{0}{1}{2}", prefix, speaker, suffix));
		}
		#endregion

		#region Title
		public virtual string Title(Post post, string datePrefix, string speakerPrefix)
		{
			return String.Format("{0}{1}{2}", post.Title, Date(post, null, datePrefix, null), Speaker(post, speakerPrefix, null));
		}
		#endregion
	}
}