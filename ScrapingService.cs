using System;
using System.Linq;
using IFRipper.Strategies;

namespace IFRipper
{
	public class ScrapingService
	{
		public ScrapingService(Options options)
		{
			Options = options;
		}

		public Options Options { get; set; }

		public void ScrapeUrl(string url, string userName, string categoryName, string galleryName)
		{
			if (Options.Exclusions.Any(p => url.Contains(p)))
			{
				Console.WriteLine($"Skipped: {url}");
				return;
			}

			ScrapingStrategyBase scrapingStrategy = null;

			if (url.Contains("profile") && url.EndsWith("galleries"))
			{
				scrapingStrategy = new UserScrapingStrategy();
			}
			else if (url.Contains("organizer"))
			{
				scrapingStrategy = new OrganizerScrapingStrategy();
			}
			else if (url.Contains("pictures"))
			{
				scrapingStrategy = new GalleryScrapingStrategy();
			}

			scrapingStrategy.ScrapeUrl(this, url, userName, categoryName, galleryName);
		}
	}
}
