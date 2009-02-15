using System;

using DnugLeipzig.Definitions.Services;

namespace DnugLeipzig.Runtime.Services
{
	public class Clock : IClock
	{
		#region Implementation of IClock
		public DateTime Now
		{
			get { return DateTime.Now; }
		}
		#endregion
	}
}