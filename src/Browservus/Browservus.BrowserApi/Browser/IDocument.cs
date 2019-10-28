using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StEn.Browservus.BrowserApi.Browser
{
	public interface IDocument : INode
	{
		Task<IElement> GetElementByIdAsync(string id);

		Task<IElement> QuerySelectorAsync(string selectors);

		Task<IEnumerable<IElement>> QuerySelectorAllAsync(string selectors);
	}
}
