using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace IFRipper.Strategies
{
	public class GalleryScrapingStrategy : ScrapingStrategyBase
	{
		public override void ScrapeUrl(ScrapingService scrapingService, string url, string userName, string categoryName, string galleryName)
		{
			if (!string.IsNullOrEmpty(scrapingService.Options.Name))
			{
				userName = scrapingService.Options.Name;
			}

			if (!string.IsNullOrEmpty(scrapingService.Options.Category))
			{
				categoryName = scrapingService.Options.Category;
			}

			Console.Write($"GALLERY {userName} / {categoryName} / {galleryName}");
			HtmlDocument htmlDoc = OpenDocument(url);

			var pictures = (IEnumerable<HtmlNode>)htmlDoc.DocumentNode.SelectNodes("//div[@id='gallery']//a");

			Parallel.ForEach(
				pictures, 
				(node) =>
				{
					string href = node.GetAttributeValue("href", string.Empty);

					if (href.StartsWith("/photo/"))
					{
						DownloadPicture(scrapingService.Options, href, userName, categoryName, galleryName);
					}
				});

			Console.WriteLine();
		}

		private void DownloadPicture(Options options, string galleryImageHRef, string userName, string categoryName, string galleryName)
		{
			try
			{
				CreateFolderStructure(options, userName, categoryName, galleryName);

				HtmlWeb web = new HtmlWeb();
				HtmlDocument htmlDoc = web.Load(string.Concat(BASE_URL, galleryImageHRef));
				HtmlNode node = htmlDoc.DocumentNode.SelectSingleNode("//img[@id='mainPhoto']");
				HtmlNode title = htmlDoc.DocumentNode.SelectSingleNode("//title");

				var fileName = TryGetFileNameFromTitle(title);

				string imageUrl = node.GetAttributeValue("src", string.Empty);

				if (string.IsNullOrEmpty(fileName))
				{
					fileName = Path.GetFileName(imageUrl);
					fileName = fileName.Substring(0, fileName.IndexOf('?'));
				}

				var absoluteFilename = GenerateAbsoluteFilePath(options, userName, categoryName, galleryName, fileName);

				if (!FileExists(absoluteFilename) ||
					FileIsZeroLength(absoluteFilename))
				{
					Console.Write("d");
					using (WebClient client = new WebClient())
					{
						client.DownloadFile(new Uri(imageUrl), absoluteFilename);
					}
				}
				else
				{
					Console.Write("s");
				}
			}
			catch (Exception ex)
			{
				Console.Write("x");				
			}
		}

		private bool FileExists(string absoluteFilename)
		{
			return File.Exists(absoluteFilename);			
		}

		private bool FileIsZeroLength(string absoluteFilename)
		{
			if (!File.Exists(absoluteFilename))
			{
				return true;
			}

			var fileInfo = new FileInfo(absoluteFilename);
			if (fileInfo.Length == 0)
			{
				return true;
			}

			return false;
		}

		private string GenerateAbsoluteFilePath(Options options, string userName, string categoryName, string galleryName, string fileName)
		{
			var userPath = Path.Combine(options.Destination, userName);
			var categoryPath = Path.Combine(userPath, categoryName);

			if (options.SuppressGalleryCreation)
			{
				return Path.Combine(categoryPath, fileName);
			}
			else
			{
				return Path.Combine(Path.Combine(categoryPath, galleryName), fileName);
			}
		}

		private string TryGetFileNameFromTitle(HtmlNode title)
		{
			var extensionIndex = title.InnerText.ToLower().IndexOf("jpg");
			var extensionLength = 3;

			if (extensionIndex == -1)
			{
				extensionIndex = title.InnerText.ToLower().IndexOf("jpeg");
				extensionLength = 4;
			}

			if (extensionIndex == -1)
			{
				extensionIndex = title.InnerText.ToLower().IndexOf("png");
			}

			if (extensionIndex == -1)
			{
				extensionIndex = title.InnerText.ToLower().IndexOf("gif");
			}

			if (extensionIndex >= 0)
			{
				return title.InnerText.Substring(0, extensionIndex + extensionLength);
			}

			return string.Empty;
		}

		private void CreateFolderStructure(Options options, string userName, string categoryName, string galleryName)
		{
			var appPath = options.Destination;
			var userPath = Path.Combine(appPath, userName);
			var categoryPath = Path.Combine(Path.Combine(appPath, userName), categoryName);
			var galleryPath = Path.Combine(Path.Combine(Path.Combine(appPath, userName), categoryName), galleryName);

			if (!Directory.Exists(appPath))
			{
				Directory.CreateDirectory(appPath);
			}

			if (!Directory.Exists(userPath))
			{
				Directory.CreateDirectory(userPath);
			}

			if (!Directory.Exists(categoryPath))
			{
				Directory.CreateDirectory(categoryPath);
			}

			if (!options.SuppressGalleryCreation &&
				!Directory.Exists(galleryPath))
			{
				Directory.CreateDirectory(galleryPath);
			}
		}
	}
}
