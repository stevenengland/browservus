using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StEn.Browservus.BrowserApi.Browser
{
	public interface IDocument : INode
	{
		Task<IHtmlElement> GetElementByIdAsync(string id);
	}
}
