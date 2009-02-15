using System.Web.Script.Serialization;

using DnugLeipzig.Definitions.Services;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Services
{
	public class EventRegistrationResult : IEventRegistrationResult
	{
		EventRegistrationResult()
		{
		}

		[ScriptIgnore]
		public Post Post
		{
			get;
			protected set;
		}

		#region IEventRegistrationResult Members
		public bool OnWaitingList
		{
			get;
			protected set;
		}

		public bool AlreadyRegistered
		{
			get;
			protected set;
		}

		public bool ErrorOccurred
		{
			get;
			protected set;
		}

		public int EventId
		{
			get
			{
				if (Post != null)
				{
					return Post.Id;
				}

				return -1;
			}
		}
		#endregion

		public static EventRegistrationResult AlreadyRegisteredFor(Post post)
		{
			return new EventRegistrationResult
			       {
			       	Post = post,
			       	AlreadyRegistered = true
			       };
		}

		public static EventRegistrationResult SuccessfullyRegisteredFor(Post post, bool onWaitingList)
		{
			return new EventRegistrationResult
			       {
			       	Post = post,
			       	OnWaitingList = onWaitingList
			       };
		}

		public static EventRegistrationResult Error()
		{
			return new EventRegistrationResult { ErrorOccurred = true };
		}

		public static EventRegistrationResult NotAllowedFor(Post post)
		{
			return new EventRegistrationResult
			       {
			       	Post = post,
			       	ErrorOccurred = true
			       };
		}
	}
}