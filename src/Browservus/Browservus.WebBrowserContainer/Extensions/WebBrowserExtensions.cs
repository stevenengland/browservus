using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StEn.Browservus.WebBrowserContainer.Extensions
{
	public static class WebBrowserExtensions
	{
		public static T SafeInvoke<T>(
			this WebBrowser webBrowser,
			Func<WebBrowser, T> call)
		{
			if (!webBrowser.Visible)
			{
				throw new InvalidOperationException("The control is not visible.");
			}

			if (!webBrowser.InvokeRequired)
			{
				return call(webBrowser);
			}

			var result = webBrowser.BeginInvoke(call, webBrowser);
			var endResult = webBrowser.EndInvoke(result);
			return (T)endResult;
		}

		public static void SafeInvoke(
			this WebBrowser webBrowser,
			Action<WebBrowser> call)
		{
			if (webBrowser.InvokeRequired)
			{
				webBrowser.BeginInvoke(call, webBrowser);
			}
			else
			{
				call(webBrowser);
			}
		}

		public static Task<bool> NavigateAsync(
			this WebBrowser webBrowser,
			string url,
			CancellationToken ct = default(CancellationToken))
		{
			return webBrowser.NavigateAsync(url, 1, ct);
		}

		public static async Task<bool> NavigateAsync(
			this WebBrowser webBrowser,
			string url,
			int pollDelay,
			CancellationToken ct = default(CancellationToken))
		{
			var uri = new Uri(url);
			pollDelay *= 1000;
			/* navigate and await DocumentCompleted */
			var tcs = new TaskCompletionSource<bool>();
			WebBrowserDocumentCompletedEventHandler handler = (s, arg) =>
			{
				tcs.TrySetResult(true);
			};

			if (ct == default(CancellationToken))
			{
				var cts = new CancellationTokenSource(30000);
				ct = cts.Token;
			}

			using (ct.Register(
				() =>
				{
					webBrowser.Stop();
					tcs.TrySetCanceled();
				},
				true))
			{
				webBrowser.DocumentCompleted += handler;
				try
				{
#pragma warning disable AsyncFixer02 // Long running or blocking operations under an async method
					webBrowser.Navigate(uri);
#pragma warning restore AsyncFixer02 // Long running or blocking operations under an async method
					await tcs.Task.ConfigureAwait(false); // wait for DocumentCompleted
				}
				finally
				{
					webBrowser.DocumentCompleted -= handler;
				}
			}

			if (webBrowser.SafeInvoke(x => x.Document?.Body == null))
			{
				return false;
			}

			/* poll the current HTML for changes asynchronously */
			var body = webBrowser.SafeInvoke(x => x.Document.GetElementsByTagName("html")[0]);

			var html = body.OuterHtml;

			while (true)
			{
				/* wait asynchronously, this will throw if cancellation requested */
				await Task.Delay(pollDelay, ct).ConfigureAwait(false);

				/* continue polling if the WebBrowser is still busy */
				if (webBrowser.SafeInvoke(x => x.IsBusy))
				{
					continue;
				}

				var htmlNow = body.OuterHtml;
				if (html == htmlNow)
				{
					break; // no changes detected, end the poll loop
				}

				html = htmlNow;
			}

			/* consider the page fully rendered  */
			ct.ThrowIfCancellationRequested();
			return true;
		}
	}
}
