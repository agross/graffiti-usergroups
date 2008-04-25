using System;
using System.Text.RegularExpressions;

using DnugLeipzig.Definitions.Extensions;

using Graffiti.Core;

namespace DnugLeipzig.Plugins
{
	internal static class Validator
	{
		/// <summary>
		///  A description of the regular expression:
		///  
		///  [1]: A numbered capture group. [[a-zA-Z0-9_\-\.]+]
		///      Any character in this class: [a-zA-Z0-9_\-\.], one or more repetitions
		///  @
		///  [2]: A numbered capture group. [(\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+)]
		///      Select from 2 alternatives
		///          [3]: A numbered capture group. [\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.]
		///              \[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.
		///                  Literal [
		///                  Any character in this class: [0-9], between 1 and 3 repetitions
		///                  Literal .
		///                  Any character in this class: [0-9], between 1 and 3 repetitions
		///                  Literal .
		///                  Any character in this class: [0-9], between 1 and 3 repetitions
		///                  Literal .
		///          [4]: A numbered capture group. [([a-zA-Z0-9\-]+\.)+]
		///              [5]: A numbered capture group. [[a-zA-Z0-9\-]+\.], one or more repetitions
		///                  [a-zA-Z0-9\-]+\.
		///                      Any character in this class: [a-zA-Z0-9\-], one or more repetitions
		///                      Literal .
		///  [6]: A numbered capture group. [[a-zA-Z]{2,4}|[0-9]{1,3}]
		///      Select from 2 alternatives
		///          Any character in this class: [a-zA-Z], between 2 and 4 repetitions
		///          Any character in this class: [0-9], between 1 and 3 repetitions
		/// </summary>
		static readonly Regex EmailRegex =
			new Regex(
				"([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})",
				RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace |
				RegexOptions.Compiled);

		public static DateTime? ValidateDate(Post post, string fieldName)
		{
			if (post[fieldName].IsNullOrEmptyTrimmed())
			{
				return null;
			}

			DateTime dateTime;
			if (!DateTime.TryParse(post[fieldName], out dateTime))
			{
				throw new ValidationException("Please enter a valid date.", fieldName);
			}

			return dateTime;
		}

		public static int? ValidateInt(Post post, string fieldName)
		{
			if (post[fieldName].IsNullOrEmptyTrimmed())
			{
				return null;
			}

			int intValue;
			if (!int.TryParse(post[fieldName], out intValue))
			{
				throw new ValidationException("Please enter a valid integer.", fieldName);
			}

			return intValue;
		}

		public static bool ValidateEmail(Post post, string fieldName)
		{
			return EmailRegex.IsMatch(post[fieldName]);
		}
	}
}