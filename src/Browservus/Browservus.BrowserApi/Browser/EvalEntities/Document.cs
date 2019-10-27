using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StEn.Browservus.BrowserApi.Browser.EvalEntities
{
	public class Document : IDocument
	{
		private readonly IJavascriptEvaluator javascriptEvaluator;

		public Document(IJavascriptEvaluator javascriptEvaluator)
		{
			this.javascriptEvaluator = javascriptEvaluator;
		}

		public string BrowservusId { get; }

		public async Task<IHtmlElement> GetElementByIdAsync(string id)
		{
			return await this.SingleNode("");
		}

		private async Task<IHtmlElement> SingleNode(string javascript)
		{
			try
			{
				var jsResponseText = await this.javascriptEvaluator.GetJavascriptResponseAsync(javascript);
				var htmlElement = new HtmlElement(this.javascriptEvaluator);
				JsonConvert.PopulateObject(jsResponseText, htmlElement);
				return htmlElement;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}
	}
}
