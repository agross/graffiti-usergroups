using System;

namespace DnugLeipzig.Definitions.Repositories
{
	public interface ICalendarItem : IHttpResponse
	{
		DateTime StartDate
		{
			get;
		}

		DateTime EndDate
		{
			get;
		}

		string Location
		{
			get;
		}

		string Subject
		{
			get;
		}

		string Description
		{
			get;
		}

		string Categories
		{
			get;
		}

		DateTime LastModified
		{
			get;
		}

		string ToString();
	}
}