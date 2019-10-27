using System;
using System.Windows.Controls;
using StEn.Browservus.BrowserApi.Browser;
using StEn.Browservus.BrowserApi.Browser.EvalEntities;

namespace StEn.Browservus.WebBrowserContainer
{
	public class WpfWebBrowser : IBrowser
	{
		private WebBrowser browser;

		public IDocument Document { get; }

		public WpfWebBrowser(WebBrowser browser)
		{
			this.browser = browser;
			//this.Document = new Document();
		}
	}
}
