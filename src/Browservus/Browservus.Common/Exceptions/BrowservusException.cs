using System;
using System.Collections.Generic;
using System.Text;

namespace StEn.Browservus.Common.Exceptions
{
	public class BrowservusException : Exception
	{
		public BrowservusException(string message)
			: base(message: message)
		{
		}

		public BrowservusException(string message, Exception ex)
			: base(message: message, innerException: ex)
		{
		}
	}
}
