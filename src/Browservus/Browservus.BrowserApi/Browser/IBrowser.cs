using System;
using System.Collections.Generic;
using System.Text;

namespace StEn.Browservus.BrowserApi.Browser
{
	public interface IBrowser
	{
		public IDocument Document { get; }
	}
}
