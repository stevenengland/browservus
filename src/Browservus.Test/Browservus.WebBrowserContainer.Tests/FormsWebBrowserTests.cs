using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using StEn.Browservus.WebBrowserContainer;
using StEn.Browservus.WebBrowserContainer.Extensions;
using StEn.Browservus.WebBrowserContainer.Utilities;
using Xunit;

namespace Browservus.WebBrowserContainer.Tests
{
	public class FormsWebBrowserTests
	{
		[Fact]
		public async Task GetElementByIdSucceeds()
		{
			using var apartment = new MessageLoopApartment();
			using var cancellationTokenSource = this.CtsFactory(20);
			/* create WebBrowser inside MessageLoopApartment */
			var webBrowser = apartment.Invoke(() => new WebBrowser());
			var container = new FormsWebBrowser(webBrowser);

			await apartment.Run(() => webBrowser.NavigateAsync(Constants.WebsiteUri, ct: cancellationTokenSource.Token), cancellationTokenSource.Token);
			apartment.Invoke(() =>
			{
				var element = Task.Run(() => container.Document.GetElementByIdAsync("divWithId")).GetAwaiter().GetResult();
				Assert.True(!string.IsNullOrWhiteSpace(element.BrowservusId)); // would throw out of apartment
			});
		}

		private CancellationTokenSource CtsFactory(int timeout = 0)
		{
			var ctTimeout = timeout * 1000;
			var cts = new CancellationTokenSource(ctTimeout);
			return cts;
		}
	}
}
