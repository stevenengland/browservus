﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace StEn.Browservus.BrowserApi.Resources
{
	public class ResourceHelper
	{
		public static string GetEmbeddedResource(string resourceName)
		{
			return GetEmbeddedResource(resourceName, Assembly.GetCallingAssembly());
		}

		public static byte[] GetEmbeddedResourceAsBytes(string resourceName)
		{
			return GetEmbeddedResourceAsBytes(resourceName, Assembly.GetCallingAssembly());
		}

		public static string GetEmbeddedResource(string resourceName, Assembly assembly)
		{
			resourceName = FormatResourceName(assembly, resourceName);
			using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
			{
				if (resourceStream == null)
					return null;

				using (StreamReader reader = new StreamReader(resourceStream))
				{
					return reader.ReadToEnd();
				}
			}
		}

		public static byte[] GetEmbeddedResourceAsBytes(string resourceName, Assembly assembly)
		{
			using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
			{
				byte[] content = new byte[resourceStream.Length];
				resourceStream.Read(content, 0, content.Length);

				return content;
			}
		}

		private static string FormatResourceName(Assembly assembly, string resourceName)
		{
			// ToDo: Include prefix automatically
			return "StEn." + assembly.GetName().Name + "." + resourceName.Replace(" ", "_").Replace("\\", ".").Replace("/", ".");
		}
	}
}
