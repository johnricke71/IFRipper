using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace IFRipper.Strategies
{
	public class OrganizerScrapingStrategy : ScrapingStrategyBase
	{		
		public override void ScrapeUrl(ScrapingService scrapingService, string url, string userName, string categoryName, string galleryName)
		{
			HtmlDocument htmlDoc = OpenDocument(url);

			var pages = ExtractPages(htmlDoc);

			if (pages.Count == 0)
			{
				ScrapOrganizerPage(scrapingService, userName, categoryName, htmlDoc);
			}
			else
			{
				foreach (string containedUrl in pages)
				{
					htmlDoc = OpenDocument(url + containedUrl);
					ScrapOrganizerPage(scrapingService, userName, categoryName, htmlDoc);
				}
			}
		}

		private void ScrapOrganizerPage(ScrapingService scrapingService, string userName, string categoryName, HtmlDocument htmlDoc)
		{
			Console.WriteLine($"ORGANIZER {userName} / {categoryName}");

			var galleries = (IEnumerable<HtmlNode>)htmlDoc.DocumentNode.SelectNodes("//a[@class='blk_galleries']");
			var processedUrls = new List<string>();

			foreach (var node in galleries)
			{
				if (node.InnerHtml.Contains("Overview") ||
					node.InnerHtml.Contains("[Show All]") ||
					node.InnerHtml.Contains("[Hide All]") ||
					!node.Attributes["href"].Value.StartsWith("/gallery/") ||
					processedUrls.Contains(node.Attributes["href"].Value))
				{
					continue;
				}				

				processedUrls.Add(node.Attributes["href"].Value);

				var responseUrl = node.Attributes["href"].Value.StartsWith(BASE_URL) ?
					GetResponseUrl(node.Attributes["href"].Value) :
					GetResponseUrl(BASE_URL + "/" + node.Attributes["href"].Value);

				scrapingService.ScrapeUrl(responseUrl + "?view=2", userName, categoryName, node.InnerText.Replace("\t", string.Empty).Replace("\n", string.Empty));
			}
		}

		private List<string> ExtractPages(HtmlDocument htmlDoc)
		{
			var results = new List<string>();
			var pageLinks = htmlDoc.DocumentNode.SelectNodes("//a[starts-with(@href, '?folderid=')]");

			foreach (var node in pageLinks)
			{
				if (node.InnerText == "Gallery Name" ||
					node.InnerText == "Images" ||
					node.InnerText == "Added" ||
					node.InnerText == "Size" ||
					node.InnerText == "Rating" ||
					node.InnerText == ":: Previous ::" ||
					node.InnerText == ":: Next ::")
				{
					continue;
				}

				results.Add(node.Attributes["href"].Value);
			}

			return results.Distinct().ToList();
		}
	}
}
