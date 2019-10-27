using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Browservus.WebBrowserContainer.Tests
{
	public static class Constants
	{
		public static string WebsiteUri { get; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + "website.html";
	}
}
