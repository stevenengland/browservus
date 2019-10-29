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

			// Ensure that ScriptErrorsSuppressed is set to false.
			browser.SafeInvoke(x => x.ScriptErrorsSuppressed = false);

			// Handle DocumentCompleted to gain access to the Document object.
			browser.SafeInvoke(x => x.DocumentCompleted +=
				new WebBrowserDocumentCompletedEventHandler(
					this.BrowserDocumentCompleted));
		}

		public IDocument Document { get; }

		public string JavascriptExecutionError { get; private set; }

		public async Task<string> GetJavascriptResponseAsync(string javascript)
		{
			string response;
			try
			{
				var jsResponseObject = await Task.Run(() => this.browser.SafeInvoke(x => x.Document.InvokeScript("eval", new[] { javascript })));
				if (jsResponseObject == null)
				{
					if (!string.IsNullOrWhiteSpace(this.JavascriptExecutionError))
					{
						throw new BrowservusException("The Javascript evaluation failed with the following error: " + this.JavascriptExecutionError);
					}

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

		private void BrowserDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			var htmlDocument = ((WebBrowser)sender).SafeInvoke(x => x.Document);
			var htmlDocumentWindow = ((WebBrowser)sender).SafeInvoke(x => x.Document?.Window);
			if (htmlDocument != null && htmlDocumentWindow != null)
			{
				htmlDocumentWindow.Error +=
					new HtmlElementErrorEventHandler(this.HandleWindowError);
			}
		}

		private void HandleWindowError(object sender, HtmlElementErrorEventArgs e)
		{
			this.JavascriptExecutionError = e.Description;

			// Ignore the error and suppress the error dialog box.
			e.Handled = true;
		}
	}
}
