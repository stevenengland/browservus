using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StEn.Browservus.BrowserApi.Browser
{
	public interface IHtmlElement : IElement
	{
		Task<string> GetInnerTextAsync();

		Task SetInnerTextAsync(string newText);
	}
}
