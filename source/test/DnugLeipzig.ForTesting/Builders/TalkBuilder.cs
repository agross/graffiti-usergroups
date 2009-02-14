using System.Web;

using DnugLeipzig.Definitions.Builders;
using DnugLeipzig.Definitions.Plugins.Talks;

using Graffiti.Core;

namespace DnugLeipzig.ForTesting.Builders
{
	public class TalkBuilder : EntityBuilder<Post>
	{
		readonly ITalkPluginConfigurationProvider _config;
		string _date;
		int _id;
		string _speaker;
		string _topic;

		public TalkBuilder(ITalkPluginConfigurationProvider configurationProvider)
		{
			_config = configurationProvider;
		}

		protected override Post BuildInstance()
		{
			Post result = new Post
			              {
			              	Id = _id,
			              	Title = HttpUtility.HtmlEncode(_topic)
			              };
			result[_config.DateField] = _date;
			result[_config.SpeakerField] = _speaker;
			return result;
		}

		public TalkBuilder Id(int id)
		{
			_id = id;
			return this;
		}

		public TalkBuilder ForDate(object date)
		{
			_date = date == null ? null : date.ToString();
			return this;
		}

		public TalkBuilder HeldBy(string speaker)
		{
			_speaker = speaker;
			return this;
		}

		public TalkBuilder TheTopicIs(string topic)
		{
			_topic = topic;
			return this;
		}
	}
}