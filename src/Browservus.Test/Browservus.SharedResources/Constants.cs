using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Browservus.SharedResources
{
	public static class Constants
	{
		private static readonly string PathToWorkingResources = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + "/HtmlResources/WorkingResources";
		private static readonly string PathToIssueResources = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + "/HtmlResources/IssueResources";

		public static void CheckFileExistence()
		{
			var props = typeof(PathToWorkingWebsites).GetProperties(BindingFlags.Public | BindingFlags.Static).ToList();
			props.AddRange(typeof(PathToIssueWebsites).GetProperties(BindingFlags.Public | BindingFlags.Static));
			if (!props.Any())
			{
				throw new Exception("Empty property info list.");
			}

			foreach (var prop in props)
			{
				var filePath = prop.GetValue(null, null).ToString().Substring(6);
				if (!File.Exists(filePath))
				{
					throw new Exception($"{prop.Name} contains no valid filepath ({filePath}).");
				}
			}
		}

		public static class PathToWorkingWebsites
		{
			public static string FullWorkingExample { get; } = PathToWorkingResources + "/website.html";
		}

		public static class PathToIssueWebsites
		{
			public static string IssueWithMissingJavascript { get; } = PathToIssueResources + "/site_without_javascript_content.html";
		}
	}
}
