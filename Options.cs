using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace IFRipper
{
	public class Options
	{
		[Option('u', "url", Required = true, HelpText = "Base Url to start scraping from.")]
		public string Url { get; set; }

		[Option('d', "dest", Required = true, HelpText = "Base folder to scrape into.")]
		public string Destination { get; set; }

		[Option('g', Required = false, Default = false, HelpText = "Suppress the Gallery folder level creation.")]
		public bool SuppressGalleryCreation { get; set; }

		[Value(1, Min = 1, Max = 3)]
		public IEnumerable<string> Exclusions { get; set; }
	}
}
