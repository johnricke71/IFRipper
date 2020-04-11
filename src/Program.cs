using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CommandLine;

namespace IFRipper
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			ServicePointManager.Expect100Continue = true;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

			var result = CommandLine.Parser.Default.ParseArguments<Options>(args).MapResult(
				(opts) => RunOptionsAndReturnExitCode(opts), 
				errs => HandleParseError(errs)); 
		}

		private static int RunOptionsAndReturnExitCode(Options options)
		{
			var exitCode = 0;

			var scrapingService = new ScrapingService(options);
			scrapingService.ScrapeUrl(options.Url, null, null, null);

			return exitCode;
		}

		private static int HandleParseError(IEnumerable<Error> errs)
		{
			var result = -2;

			Console.WriteLine("errors {0}", errs.Count());

			if (errs.Any(x => x is HelpRequestedError || x is VersionRequestedError))
			{
				result = -1;
			}

			Console.WriteLine("Exit code {0}", result);
			Console.ReadKey();

			return result;
		}
	}
}