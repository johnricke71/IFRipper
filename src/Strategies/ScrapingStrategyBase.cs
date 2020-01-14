using System;
using HtmlAgilityPack;

namespace IFRipper.Strategies
{
	public abstract class ScrapingStrategyBase
	{
		public const string BASE_URL = "https://www.imagefap.com";

		public virtual void ScrapeUrl(ScrapingService scrapingService, string url, string userName, string categoryName, string galleryName)
		{
		}

		public HtmlDocument OpenDocument(string url)
		{
			HtmlDocument htmlDoc = null;

			try
			{
				htmlDoc = (new HtmlWeb()).Load(url);
				HtmlNode title = htmlDoc.DocumentNode.SelectSingleNode("//title");
			}
			catch (Exception)
			{
				Console.WriteLine($"ERROR OpenDocument: [{url}]");
			}

			return htmlDoc;
		}

		public string GetResponseUrl(string url)
		{
			var client = new HtmlWeb();

			client.Load(url);

			return client.ResponseUri.AbsoluteUri;
		}
	}
}
