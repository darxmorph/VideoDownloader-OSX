using System;
using System.ComponentModel;

namespace Formula1Downloader.Downloaders
{
	public abstract class Downloader
	{
		public Video Video { get; protected set; }
		public string OutputFile { get; protected set; }

		public event EventHandler DownloadComplete;
		public event ProgressChangedEventHandler ProgressChanged;

		public abstract void StartDownload();

		protected virtual void OnDownloadComplete(EventArgs e)
		{
			DownloadComplete?.Invoke(this, e);
		}

		protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
		{
			ProgressChanged?.Invoke(this, e);
		}
	}
}