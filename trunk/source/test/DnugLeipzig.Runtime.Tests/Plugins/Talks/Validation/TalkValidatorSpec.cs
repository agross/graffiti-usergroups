using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Plugins.Talks;
using DnugLeipzig.Definitions.Validation;
using DnugLeipzig.ForTesting;
using DnugLeipzig.Runtime.Plugins.Talks.Validation;

using Graffiti.Core;

using MbUnit.Framework;

namespace DnugLeipzig.Runtime.Tests.Plugins.Talks.Validation
{
	public class When_a_talk_is_validated : Spec
	{
		ValidationReport _report;
		ITalkValidator _sut;

		protected override void Establish_context()
		{
			_sut = new TalkValidator(Create.New.StubbedTalkPluginConfiguration().Build());
		}

		void Because(Post post)
		{
			_report = _sut.Validate(post);
		}

		[RowTest]
		[Row("")]
		[Row("     ")]
		[Row(null)]
		[Row("2008/2/3")]
		[Row("2008/2/3 8:00 AM")]
		[Row("3.2.2008 8:00")]
		public void It_should_accept_valid_talk_dates(string dateValue)
		{
			Because(Create.New.Talk()
				.ForDate(dateValue));

			Assert.AreEqual(0, _report.Count);
		}

		[RowTest]
		[Row("invalid value")]
		public void It_should_reject_invalid_talk_dates(string dateValue)
		{
			Because(Create.New.Talk()
				.ForDate(dateValue));

			Assert.AreEqual(1, _report.Count);
		}
	}
}