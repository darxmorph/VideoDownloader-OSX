using System;

using System.Collections.Generic;
using HtmlAgilityPack;

namespace Formula1Downloader
{
	public static class F1Utils
	{
		public static List<Video> GetVideosFromUri(Uri videoURI)
		{
			List<Video> vids = new List<Video>();

			HtmlWeb web = new HtmlWeb();
			HtmlDocument htmlDoc = web.Load(videoURI.AbsoluteUri);
			HtmlNodeCollection mdNodeCol = htmlDoc.DocumentNode.SelectNodes("//div[@data-videoid]");

			if (mdNodeCol != null)
			{
				foreach (HtmlNode mdnode in mdNodeCol)
				{
					HtmlAttribute desc = mdnode.Attributes["data-videoid"];
					vids.Add(new Video(desc.Value));
				}
			}

			return vids;
		}
	}
}