using System;
using System.Text.RegularExpressions;

using Castle.Core.Logging;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Logging
{
	public class GraffitiLogger : ILogger
	{
		/// <summary>
		///  Select from 2 alternatives
		///      ^(?<Title>.*?)\|(?<Message>.*)
		///          Beginning of line or string
		///          [Title]: A named capture group. [.*?]
		///              Any character, any number of repetitions, as few as possible
		///          Literal |
		///          [Message]: A named capture group. [.*]
		///              Any character, any number of repetitions
		///      (?<Message>.+)$
		///          [Message]: A named capture group. [.+]
		///              Any character, one or more repetitions
		///          End of line or string
		/// </summary>
		static readonly Regex TitleAndMessage = new Regex("^(?<Title>.*?)\\|(?<Message>.*)|(?<Message>.+)$",
		                                                  RegexOptions.IgnoreCase
		                                                  | RegexOptions.Singleline
		                                                  | RegexOptions.ExplicitCapture
		                                                  | RegexOptions.CultureInvariant
		                                                  | RegexOptions.IgnorePatternWhitespace
		                                                  | RegexOptions.Compiled);

		readonly string _name;

		public GraffitiLogger(string name)
		{
			_name = name;
		}

		static string TitleOf(string message)
		{
			if (String.IsNullOrEmpty(message))
			{
				return String.Empty;
			}

			return TitleAndMessage.Match(message).Groups["Title"].Value;
		}

		string MessageOf(string message)
		{
			if (String.IsNullOrEmpty(message))
			{
				return String.Empty;
			}

			return String.Format("{0}{1}[{2}]",
			                     TitleAndMessage.Match(message).Groups["Message"].Value,
			                     Environment.NewLine,
			                     _name);
		}

		#region Implementation of ILogger
		public void Debug(string message)
		{
			Info(message);
		}

		public void Debug(string message, Exception exception)
		{
			Info(message, exception);
		}

		public void Debug(string format, params object[] args)
		{
			Info(format, args);
		}

		public void Info(string message)
		{
			Log.Info(TitleOf(message), MessageOf(message));
		}

		public void Info(string message, Exception exception)
		{
			Log.Info(TitleOf(message), MessageOf(message) + Environment.NewLine + exception);
		}

		public void Info(string format, params object[] args)
		{
			string message = String.Format(format, args);
			Log.Info(TitleOf(message), MessageOf(message));
		}

		public void Warn(string message)
		{
			Log.Warn(TitleOf(message), MessageOf(message));
		}

		public void Warn(string message, Exception exception)
		{
			Log.Warn(TitleOf(message), MessageOf(message) + Environment.NewLine + exception);
		}

		public void Warn(string format, params object[] args)
		{
			string message = String.Format(format, args);
			Log.Warn(TitleOf(message), MessageOf(message));
		}

		public void Error(string message)
		{
			Log.Error(TitleOf(message), MessageOf(message));
		}

		public void Error(string message, Exception exception)
		{
			Log.Error(TitleOf(message), MessageOf(message) + Environment.NewLine + exception);
		}

		public void Error(string format, params object[] args)
		{
			string message = String.Format(format, args);
			Log.Error(TitleOf(message), MessageOf(message));
		}

		public void Fatal(string message)
		{
			Error(message);
		}

		public void Fatal(string message, Exception exception)
		{
			Error(message, exception);
		}

		public void Fatal(string format, params object[] args)
		{
			Error(format, args);
		}

		public void FatalError(string message)
		{
			Error(message);
		}

		public void FatalError(string message, Exception exception)
		{
			Error(message, exception);
		}

		public void FatalError(string format, params object[] args)
		{
			Error(format, args);
		}

		public ILogger CreateChildLogger(string loggerName)
		{
			throw new NotSupportedException();
		}

		public bool IsDebugEnabled
		{
			get { return true; }
		}

		public bool IsInfoEnabled
		{
			get { return true; }
		}

		public bool IsWarnEnabled
		{
			get { return true; }
		}

		public bool IsErrorEnabled
		{
			get { return true; }
		}

		public bool IsFatalEnabled
		{
			get { return true; }
		}

		public bool IsFatalErrorEnabled
		{
			get { return true; }
		}
		#endregion

		#region Not Supported
		public void DebugFormat(string format, params object[] args)
		{
			throw new NotSupportedException();
		}

		public void DebugFormat(Exception exception, string format, params object[] args)
		{
			throw new NotSupportedException();
		}

		public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotSupportedException();
		}

		public void DebugFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotSupportedException();
		}

		public void InfoFormat(string format, params object[] args)
		{
			throw new NotSupportedException();
		}

		public void InfoFormat(Exception exception, string format, params object[] args)
		{
			throw new NotSupportedException();
		}

		public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotSupportedException();
		}

		public void InfoFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotSupportedException();
		}

		public void WarnFormat(string format, params object[] args)
		{
			throw new NotSupportedException();
		}

		public void WarnFormat(Exception exception, string format, params object[] args)
		{
			throw new NotSupportedException();
		}

		public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotSupportedException();
		}

		public void WarnFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotSupportedException();
		}

		public void ErrorFormat(string format, params object[] args)
		{
			throw new NotSupportedException();
		}

		public void ErrorFormat(Exception exception, string format, params object[] args)
		{
			throw new NotSupportedException();
		}

		public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotSupportedException();
		}

		public void ErrorFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotSupportedException();
		}

		public void FatalFormat(string format, params object[] args)
		{
			throw new NotSupportedException();
		}

		public void FatalFormat(Exception exception, string format, params object[] args)
		{
			throw new NotSupportedException();
		}

		public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotSupportedException();
		}

		public void FatalFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotSupportedException();
		}
		#endregion
	}
}