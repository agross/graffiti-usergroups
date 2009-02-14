using System;

namespace DnugLeipzig.ForTesting
{
	public static class Catch
	{
		public static Exception Exception(Action throwingAction)
		{
			try
			{
				throwingAction();
			}
			catch (Exception exception)
			{
				return exception;
			}

			return null;
		}
	}
}