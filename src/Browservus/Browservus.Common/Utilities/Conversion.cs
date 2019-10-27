using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

namespace StEn.Browservus.Common.Utilities
{
	public static class Conversion
	{
		public static string YamlToJson(string yamlText)
		{
			// convert string/file to YAML object
			var r = new StringReader(yamlText);
			var deserializer = new DeserializerBuilder().Build();
			var yamlObject = deserializer.Deserialize(r);

			var serializer = new SerializerBuilder()
				.JsonCompatible()
				.Build();

			var json = serializer.Serialize(yamlObject);

			return json;
		}
	}
}
