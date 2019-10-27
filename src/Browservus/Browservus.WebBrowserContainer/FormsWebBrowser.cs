using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nito.AsyncEx.Synchronous;
using StEn.Browservus.BrowserApi.Browser;
using StEn.Browservus.BrowserApi.Browser.EvalEntities;
using StEn.Browservus.WebBrowserContainer.Extensions;

namespace StEn.Browservus.WebBrowserContainer
{
	public class FormsWebBrowser : IBrowser, IJavascriptEvaluator
	{
		private readonly WebBrowser browser;

		public FormsWebBrowser(WebBrowser browser)
		{
			this.browser = browser;
			this.Document = new Document(this);
		}

		public IDocument Document { get; }

		public async Task<string> GetJavascriptResponseAsync(string javascript)
		{
			// mapping machen die aufrufenden Funktionen, hier wird nur string zurückgeliefert.
			// hier kann error handling hinein.

			var response = string.Empty;
			try
			{
				var jsResponse = await Task.Run(() => this.browser.SafeInvoke(x => x.Document.InvokeScript("eval", new[] { javascript })).ToString());
			}
			catch
			{
			}

			return response;
		}
	}
}
