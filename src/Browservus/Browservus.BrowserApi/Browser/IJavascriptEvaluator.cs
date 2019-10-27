using System.Threading.Tasks;

namespace StEn.Browservus.BrowserApi.Browser
{
	public interface IJavascriptEvaluator
	{
		Task<string> GetJavascriptResponseAsync(string javascript);
	}
}
