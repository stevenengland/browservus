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

		public async Task<IElement> GetElementByIdAsync(string id)
		{
			var replacements = new Dictionary<string, string>()
			{
				{ "_ID_", id },
			};
			return await this.SingleNode(ResourceHelper.GetEmbeddedResource(this.pathToJsEvalScriptsRoot + "eval_getElementById.js"), replacements);
		}

		public async Task<IElement> QuerySelectorAsync(string selectors)
		{
			var replacements = new Dictionary<string, string>()
			{
				{ "_SELECTORS_", selectors },
			};
			return await this.SingleNode(ResourceHelper.GetEmbeddedResource(this.pathToJsEvalScriptsRoot + "eval_querySelector.js"), replacements);
		}

		public async Task<IEnumerable<IElement>> QuerySelectorAllAsync(string selectors)
		{
			throw new NotImplementedException();
		}

		private async Task<IElement> SingleNode(string javascript, Dictionary<string, string> replacements)
		{
			try
			{
				var callerId = this.GetId();
				replacements.Add("_CALLERID_", callerId);
				foreach (var key in replacements)
				{
					javascript = javascript.Replace(key.Key, key.Value);
				}

				var element = new Element(callerId, this.javascriptEvaluator);
				var jsResponseText = await this.javascriptEvaluator.GetJavascriptResponseAsync(javascript);
				if (string.IsNullOrWhiteSpace(jsResponseText))
				{
					throw new BrowservusException("The response of the Javascript evaluation returned an empty answer without a hint to an actual error.");
				}

				var jsResponse = JsonConvert.DeserializeObject<EvalResponse<string>>(jsResponseText);
				if (!jsResponse.IsSuccess)
				{
					throw new BrowservusException(jsResponse.ErrorMessage);
				}

				return element;
			}
			catch (JsonSerializationException ex)
			{
				throw new BrowservusException("The Javascript evaluation returned an invalid structure.", ex);
			}
			catch (Exception ex) when (!(ex is BrowservusException))
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
