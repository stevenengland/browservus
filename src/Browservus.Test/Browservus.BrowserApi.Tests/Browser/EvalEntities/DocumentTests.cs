using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using StEn.Browservus.BrowserApi.Browser;
using StEn.Browservus.BrowserApi.Browser.EvalEntities;
using Xunit;

namespace Browservus.BrowserApi.Tests.Browser.EvalEntities
{
	public class DocumentTests
	{
		[Fact]
		public async Task GetElementById()
		{
			var mock = new Mock<IJavascriptEvaluator>();
			var document = new Document(mock.Object);
		}
	}
}
