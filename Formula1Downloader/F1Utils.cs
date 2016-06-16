using System;

using AppKit;
using Foundation;

using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using hdsdump;

namespace Formula1Downloader
{
	public static class F1Utils
	{
		public static Video[] getF4MManifestURLsFromVideoURI(Uri videoURI) {
			List<Video> vids = new List<Video> ();

			HtmlWeb web = new HtmlWeb();
			HtmlDocument htmlDoc = web.Load(videoURI.AbsoluteUri);

			HtmlNodeCollection mdNodeCol = htmlDoc.DocumentNode.SelectNodes("//div[@data-videoid]");

			if (mdNodeCol != null)
			{
				if (mdNodeCol.Count > 1)
				{
					foreach (HtmlNode mdnode in mdNodeCol)
					{
						HtmlAttribute desc = mdnode.Attributes["data-videoid"];

						HtmlNodeCollection titleNodes = mdnode.ParentNode.PreviousSibling.PreviousSibling.SelectNodes("h5");

						// Some vids use h4

						if (titleNodes == null)
						{
							titleNodes = mdnode.ParentNode.PreviousSibling.PreviousSibling.SelectNodes("h4");
						}

						string videoId = desc.Value;
						string videoTitle = "";

						foreach (HtmlNode titleNode in titleNodes)
						{
							string text = titleNode.InnerText.Trim();
							if (text != "")
							{
								videoTitle = text;
							}
						}

						vids.Add (new Video (videoTitle, videoId, getF4MManifestURLForVideoId (videoId)));
					}
				}
				else {
					HtmlNode mdnode = mdNodeCol.First();
					HtmlAttribute desc = mdnode.Attributes["data-videoid"];
					string videoId = desc.Value;
					HtmlNode titlenode = htmlDoc.DocumentNode.SelectSingleNode("//h1");
					string videoTitle = titlenode.InnerText;

					vids.Add(new Video(videoTitle, videoId, getF4MManifestURLForVideoId(videoId)));
				}
			}
			else {
				// We should do something in the UI about this null
				return null;
			}

			return vids.ToArray();
		}

		public static F1VideoTypes? getF1UriVideoType(Uri videoURI)
		{
			HtmlWeb web = new HtmlWeb();
			HtmlAgilityPack.HtmlDocument htmlDoc = web.Load(videoURI.AbsoluteUri);

			HtmlNodeCollection mdNodeCol = htmlDoc.DocumentNode.SelectNodes("//div[@data-videoid]");

			if (mdNodeCol != null)
			{
				if (mdNodeCol.Count > 1)
				{
					return F1VideoTypes.H5AndVideo;
				}
				else {
					return F1VideoTypes.SingleVideo;
				}
			}

			return null;
		}

		public enum F1VideoTypes {
			SingleVideo,
			H5AndVideo,
		}

		internal static string getF4MManifestURLForVideoId(string videoId) {
			return string.Format("http://f1.pc.cdn.bitgravity.com/{0}/{0}_1.f4m", videoId);
		}

		public static void getVideoUsingAdobeHDS(Video video, string outFile, NSProgressIndicator ProgressBar)
		{
			F4F f4f = new F4F();
			f4f.quality = "3600";
			f4f.outPath = outFile;
			f4f.DownloadFragments(video.ManifestUrl, ProgressBar);
		}
	}
}