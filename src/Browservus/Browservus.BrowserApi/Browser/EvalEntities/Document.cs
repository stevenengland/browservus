using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StEn.Browservus.BrowserApi.Resources;
using StEn.Browservus.Common.Exceptions;
using StEn.Browservus.Common.Utilities;

namespace StEn.Browservus.BrowserApi.Browser.EvalEntities
{
	public class Document : IDocument
	{
		private readonly IJavascriptEvaluator javascriptEvaluator;

		private readonly string pathToJsEvalScriptsRoot = "Resources/Javascript/Eval/";

		public Document(IJavascriptEvaluator javascriptEvaluator)
		{
			this.javascriptEvaluator = javascriptEvaluator;
		}

		public string BrowservusId { get; }

		public async Task<IHtmlElement> GetElementByIdAsync(string id)
		{
			var replacements = new Dictionary<string, string>()
			{
				{ "_ID_", id },
			};
			return await this.SingleNode(ResourceHelper.GetEmbeddedResource(this.pathToJsEvalScriptsRoot + "eval_getElementById.js"), replacements);
		}

		private async Task<IHtmlElement> SingleNode(string javascript, Dictionary<string, string> replacements)
		{
			try
			{
				var callerId = this.GetId();
				replacements.Add("_CALLERID_", callerId);
				foreach (var key in replacements)
				{
					javascript = javascript.Replace(key.Key, key.Value);
				}

				var htmlElement = new HtmlElement(callerId, this.javascriptEvaluator);
				var jsResponseText = await this.javascriptEvaluator.GetJavascriptResponseAsync(javascript);
				var jsResponse = JsonConvert.DeserializeObject<EvalResponse<string>>(jsResponseText);
				if (!jsResponse.IsSuccess)
				{
					throw new BrowservusException(jsResponse.ErrorMessage);
				}

				// JsonConvert.PopulateObject(jsResponseText, htmlElement);
				return htmlElement;
			}
			catch (JsonSerializationException ex)
			{
				throw new BrowservusException("The Javascript evaluation returned an invalid structure.", ex);
			}
			catch (Exception ex)
			{
				throw new BrowservusException("An unknown error occured when evaluating Javascript.", ex);
			}
		}

		private string GetId()
		{
			return "Browservus_" + IDGenerator.Instance.Next;
		}
	}
}
