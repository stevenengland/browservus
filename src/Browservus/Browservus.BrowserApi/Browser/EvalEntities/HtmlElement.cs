using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace StEn.Browservus.BrowserApi.Browser.EvalEntities
{
	public class HtmlElement : IHtmlElement
	{
		private IJavascriptEvaluator javascriptEvaluator;

		public HtmlElement(IJavascriptEvaluator javascriptEvaluator)
		{
			this.javascriptEvaluator = javascriptEvaluator;
		}

		public string BrowservusId { get; }

		public async Task<string> GetInnerTextAsync()
		{
			return await this.javascriptEvaluator.GetJavascriptResponseAsync("");
		}

		public async Task SetInnerTextAsync(string newText)
		{
			await this.javascriptEvaluator.GetJavascriptResponseAsync("");
		}
	}
}
