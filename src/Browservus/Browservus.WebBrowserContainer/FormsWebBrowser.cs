using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nito.AsyncEx.Synchronous;
using StEn.Browservus.BrowserApi.Browser;
using StEn.Browservus.BrowserApi.Browser.EvalEntities;
using StEn.Browservus.Common.Exceptions;
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
			string response;
			try
			{
				var jsResponseObject = await Task.Run(() => this.browser.SafeInvoke(x => x.Document.InvokeScript("eval", new[] { javascript })));
				if (jsResponseObject == null)
				{
					throw new BrowservusException("The evaluation of Javascript returned null. One possible reason is when there is no Javascript content within the page that was loaded (e.g. not a single <script> tag was found).");
				}

				response = jsResponseObject.ToString();
			}

			// If 'Document' is not available
			catch (NullReferenceException)
			{
				throw new BrowservusException("Cannot invoke Javascript, because the browser document is null.");
			}

			return response;
		}
	}
}
