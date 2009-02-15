using System;

namespace DnugLeipzig.Definitions.Services
{
	public interface IClock
	{
		DateTime Now
		{
			get;
		}
	}
}