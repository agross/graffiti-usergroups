using System;
using System.Collections;
using System.Text;
using System.Web;

using Graffiti.Core;

namespace DnugLeipzig.Extensions
{
	[Chalk("dnug")]
	public class Dnug
	{
		public string EventDate(Post post, string prefix, string suffix, string datePattern)
		{
			if (!Util.IsEvent(post))
			{
				return String.Empty;
			}

			DateTime date;
			if (!DateTime.TryParse(post.CustomFields()["Datum"], out date))
			{
				return String.Empty;
			}

			return String.Format("{0}{1}{2}", prefix, HttpUtility.HtmlEncode(date.ToString(datePattern)), suffix);
		}

		public string EventSpeaker(Post post, string prefix, string suffix)
		{
			if (!Util.IsEvent(post))
			{
				return String.Empty;
			}

			string speaker = post.CustomFields()["Sprecher"];
			if (speaker == null || speaker.Trim().Length == 0)
			{
				return String.Empty;
			}

			return String.Format("{0}{1}{2}", prefix, HttpUtility.HtmlEncode(speaker), suffix);
		}

		public string EventTitle(Post post, string datePrefix, string speakerPrefix)
		{
			return
				HttpUtility.HtmlAttributeEncode(String.Format("{0}{1}{2}",
				                                              post.Title,
				                                              EventDate(post, datePrefix, String.Empty, "d"),
				                                              EventSpeaker(post, speakerPrefix, String.Empty)));
		}

		public string CommentUrl(Post post, IDictionary dictionary)
		{
			if (post == null)
			{
				return String.Empty;
			}

			string anchor = null;
			if (dictionary != null)
			{
				if (dictionary.Contains("anchor"))
				{
					anchor = String.Format("#{0}", HttpUtility.HtmlAttributeEncode(dictionary["anchor"] as string));
					dictionary.Remove("anchor");
				}
			}

			string linkText;
			if (post.CommentCount <= 0)
			{
				linkText = "Keine Kommentare";
			}
			else if (post.CommentCount == 1)
			{
				linkText = "1 Kommentar";
			}
			else
			{
				linkText = String.Format("{0} Kommentare", post.CommentCount);
			}

			return String.Format("<a href=\"{0}{1}{2}\">{3}</a>",
			                     HttpUtility.HtmlAttributeEncode(post.Url),
			                     DictionaryToQueryString(dictionary),
			                     anchor,
			                     HttpUtility.HtmlEncode(linkText));
		}

		public string CategoryLink(Category category, string prefix)
		{
			if (category == null)
			{
				return String.Empty;
			}

			if (category.Id != 1)
			{
				return String.Format("{0}<a href=\"{1}\">{2}</a>",
				                     HttpUtility.HtmlEncode(prefix),
				                     HttpUtility.HtmlAttributeEncode(category.Url),
				                     category.Name);
			}

			return String.Format("{0}(keine)", HttpUtility.HtmlEncode(prefix));
		}

		public string TagList(string tagList, string prefix)
		{
			if (String.IsNullOrEmpty(tagList))
			{
				return string.Empty;
			}

			string[] tags = tagList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			string[] result = new string[tags.Length];
			for (int i = 0; i < tags.Length; i++)
			{
				result[i] = String.Format("<a href=\"{0}/\" rel=\"tag\">{1}</a>",
				                          HttpUtility.HtmlAttributeEncode(VirtualPathUtility.ToAbsolute("~/tags/") +
				                                                          Graffiti.Core.Util.CleanForUrl(tags[i])),
				                          HttpUtility.HtmlEncode(tags[i]));
			}

			return String.Format("{0}{1}", HttpUtility.HtmlEncode(prefix), String.Join(", ", result));
		}

		static string DictionaryToQueryString(IDictionary dictionary)
		{
			if (dictionary == null || dictionary.Count == 0)
			{
				return null;
			}

			StringBuilder result = new StringBuilder("?");

			foreach (DictionaryEntry entry in dictionary)
			{
				result.AppendFormat("{0}={1}&",
				                    HttpUtility.HtmlAttributeEncode(entry.Key as string),
				                    HttpUtility.HtmlAttributeEncode(entry.Value as string));
			}

			// Strip the last ampersand.
			return result.ToString(0, result.Length - 1);
		}
	}
}