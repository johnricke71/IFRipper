using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace IFRipper.Strategies
{
	public class UserScrapingStrategy : ScrapingStrategyBase
	{
		public override void ScrapeUrl(ScrapingService scrapingService, string url, string userName, string categoryName, string galleryName)
		{
			var startIndex = url.IndexOf("profile/") + 8;
			var length = url.IndexOf("/galleries") - startIndex;

			userName = url.Substring(startIndex, length);

			Console.WriteLine($"USER {userName}");

			HtmlDocument htmlDoc = OpenDocument(url);

			var galleries = (IEnumerable<HtmlNode>)htmlDoc.DocumentNode.SelectNodes("//a[@class='blk_galleries']");

			foreach (var node in galleries)
			{
				if (node.InnerHtml.Contains("<b>Overview</b>") ||
					node.InnerHtml.Contains("[Show All]") ||
					node.InnerHtml.Contains("[Hide All]"))
				{
					continue;
				}

				var responseUrl = GetResponseUrl(node.Attributes["href"].Value);

				scrapingService.ScrapeUrl(responseUrl, userName, node.InnerText, null);
			}
		}
	}
}
