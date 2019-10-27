using StEn.Browservus.Common.Utilities;
using System;
using Xunit;

namespace Browservus.Common.Tests
{
	public class ConversionTests
	{
		[Fact]
		public void YamlIsConvertedToJson()
		{
			var yamlText = @"
scalar: a scalar
sequence:
  - one
  - two";
			var jsonText = Conversion.YamlToJson(yamlText);
			Assert.Contains("\"scalar\":", jsonText);
		}
	}
}
