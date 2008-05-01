using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web;

using DnugLeipzig.Definitions.Extensions;

using Graffiti.Core;

namespace DnugLeipzig.Extensions.Macros
{
	// TODO: Refactor into a plugin and let the user define the values of the various fields using the Graffiti UI.
	[Chalk("ui")]
	public class UiMacros
	{
		static readonly Graffiti.Core.Macros GraffitiMacros = new Graffiti.Core.Macros();
		static readonly string ManyComments;
		static readonly string NoComments;
		static readonly string SingleComment;

		static UiMacros()
		{
			// TODO: German
			// Not perfect as there may be languages where there's no simple singular/plural forms.
			NoComments = ConfigurationManager.AppSettings.GetOrDefault("Ui:Comments:NoComments", "Keine Kommentare");
			SingleComment = ConfigurationManager.AppSettings.GetOrDefault("Ui:Comments:SingleComment", "1 Kommentar");
			ManyComments = ConfigurationManager.AppSettings.GetOrDefault("Ui:Comments:ManyComments", "{0} Kommentare");
		}

		public bool IsFirstNavigationLinkSelected()
		{
			List<Link> links = GraffitiMacros.NavigationLinks();
			return (links.Count > 0 && links[0].IsSelected);
		}

		public bool IsNavigationLinkSelected()
		{
			List<Link> links = GraffitiMacros.NavigationLinks();

			return Array.Exists(links.ToArray(), link => link.IsSelected);
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
				linkText = NoComments;
			}
			else if (post.CommentCount == 1)
			{
				linkText = SingleComment;
			}
			else
			{
				linkText = String.Format(ManyComments, post.CommentCount);
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
				return String.Empty;
			}

			string[] tags = tagList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			var result = new string[tags.Length];
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

			var result = new StringBuilder("?");

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