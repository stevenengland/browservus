using System;
using System.Collections.Generic;
using System.Text;

namespace StEn.Browservus.BrowserApi.Browser.EvalEntities
{
	public class Element : IElement
	{
		private IJavascriptEvaluator javascriptEvaluator;

		public Element(string id, IJavascriptEvaluator javascriptEvaluator)
		{
			this.javascriptEvaluator = javascriptEvaluator;
			this.BrowservusId = id;
		}

		public string BrowservusId { get; }
	}
}
