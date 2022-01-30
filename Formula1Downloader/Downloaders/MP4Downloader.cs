using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;

namespace Formula1Downloader.Downloaders
{
	public class MP4Downloader : Downloader
	{
		public MP4Downloader(Video video, string outFile)
		{
			Video = video;
			OutputFile = outFile;
		}

		public override void StartDownload()
		{
			using (var webClient = new WebClient())
			{
				webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadCompleted);
				webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressChanged);

				webClient.DownloadFileAsync(Video.Uri, OutputFile);
			}
		}

		private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			OnProgressChanged(e);
		}

		private void DownloadCompleted(object sender, AsyncCompletedEventArgs e)
		{
			OnDownloadComplete(e);
		}
	}
}