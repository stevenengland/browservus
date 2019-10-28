using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Browservus.SharedResources;
using StEn.Browservus.Common.Exceptions;
using StEn.Browservus.WebBrowserContainer;
using StEn.Browservus.WebBrowserContainer.Extensions;
using StEn.Browservus.WebBrowserContainer.Utilities;
using Xunit;

namespace Browservus.WebBrowserContainer.Tests
{
	public class FormsWebBrowserTests
	{
		public FormsWebBrowserTests()
		{
			Constants.CheckFileExistence();
		}

		[Fact]
		public async Task GetElementByIdSucceedsIfItemWasFoundAsync()
		{
			using var apartment = new MessageLoopApartment();
			using var cancellationTokenSource = this.CtsFactory(20);
			/* create WebBrowser inside MessageLoopApartment */
			var webBrowser = apartment.Invoke(() => new WebBrowser());
			var container = new FormsWebBrowser(webBrowser);

			await apartment.Run(() => webBrowser.NavigateAsync(Constants.PathToWorkingWebsites.FullWorkingExample, ct: cancellationTokenSource.Token), cancellationTokenSource.Token);
			apartment.Invoke(() =>
			{
				var test = webBrowser.SafeInvoke(x => x.DocumentText); // would throw out of apartment
			});
			var element = await container.Document.GetElementByIdAsync("divWithId");
			Assert.True(!string.IsNullOrWhiteSpace(element.BrowservusId)); 
		}

		[Fact]
		public async Task GetElementByIdFailsIfItemWasNotFoundAsync()
		{
			using var apartment = new MessageLoopApartment();
			using var cancellationTokenSource = this.CtsFactory(20);
			/* create WebBrowser inside MessageLoopApartment */
			var webBrowser = apartment.Invoke(() => new WebBrowser());
			var container = new FormsWebBrowser(webBrowser);

			await apartment.Run(() => webBrowser.NavigateAsync(Constants.PathToWorkingWebsites.FullWorkingExample, ct: cancellationTokenSource.Token), cancellationTokenSource.Token);

			await Assert.ThrowsAsync<BrowservusException>(() => container.Document.GetElementByIdAsync("someWeirdIdThatIsNeverUsed"));
		}

		[Fact]
		public async Task QuerySelectorSucceedsIfItemWasFoundAsync()
		{
			using var apartment = new MessageLoopApartment();
			using var cancellationTokenSource = this.CtsFactory(20);
			/* create WebBrowser inside MessageLoopApartment */
			var webBrowser = apartment.Invoke(() => new WebBrowser());
			var container = new FormsWebBrowser(webBrowser);

			await apartment.Run(() => webBrowser.NavigateAsync(Constants.PathToWorkingWebsites.FullWorkingExample, ct: cancellationTokenSource.Token), cancellationTokenSource.Token);

			var element = await container.Document.GetElementByIdAsync(".querySelectorClass");
			Assert.True(!string.IsNullOrWhiteSpace(element.BrowservusId));
		}

		[Fact]
		public async Task QuerySelectorFailsIfItemWasNotFoundAsync()
		{
			using var apartment = new MessageLoopApartment();
			using var cancellationTokenSource = this.CtsFactory(20);
			/* create WebBrowser inside MessageLoopApartment */
			var webBrowser = apartment.Invoke(() => new WebBrowser());
			var container = new FormsWebBrowser(webBrowser);

			await apartment.Run(() => webBrowser.NavigateAsync(Constants.PathToWorkingWebsites.FullWorkingExample, ct: cancellationTokenSource.Token), cancellationTokenSource.Token);

			await Assert.ThrowsAsync<BrowservusException>(() => container.Document.GetElementByIdAsync("someWeirdQuerySelectorThatIsNeverSucceeding"));
		}

		#region IssueTests

		[Fact]
		public async Task EvalJavascriptFailsIfWebsiteDoesNotContainAnyJavascriptAsync()
		{
			using var apartment = new MessageLoopApartment();
			using var cancellationTokenSource = this.CtsFactory(20);
			/* create WebBrowser inside MessageLoopApartment */
			var webBrowser = apartment.Invoke(() => new WebBrowser());
			var container = new FormsWebBrowser(webBrowser);
			// If the website does not contain any Javascript it will fail to evaluate new Javascript. 
			await apartment.Run(() => webBrowser.NavigateAsync(Constants.PathToIssueWebsites.IssueWithMissingJavascript, ct: cancellationTokenSource.Token), cancellationTokenSource.Token);
			await Assert.ThrowsAsync<BrowservusException>(() => container.Document.GetElementByIdAsync("divWithId")); 
		}

		#endregion

		private CancellationTokenSource CtsFactory(int timeout = 0)
		{
			var ctTimeout = timeout * 1000;
			var cts = new CancellationTokenSource(ctTimeout);
			return cts;
		}
	}
}
