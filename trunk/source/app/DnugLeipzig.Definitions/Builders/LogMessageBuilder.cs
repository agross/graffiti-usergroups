using System;

namespace DnugLeipzig.Definitions.Builders
{
	public class LogMessageBuilder : EntityBuilder<string>
	{
		const string TitleAndMessageSeparator = "|";
		string _message;
		string _title;

		public LogMessageBuilder WithMessage(string message, params object[] args)
		{
			_message = String.Format(message, args);
			return this;
		}

		public LogMessageBuilder WithTitle(string title, params object[] args)
		{
			_title = String.Format(title, args);
			return this;
		}

		#region Overrides of EntityBuilder<string>
		protected override string BuildInstance()
		{
			return String.Format("{0}{1}{2}", _title, TitleAndMessageSeparator, _message);
		}
		#endregion
	}
}