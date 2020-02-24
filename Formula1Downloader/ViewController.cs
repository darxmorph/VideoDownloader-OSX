using System;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using AppKit;
using Foundation;
using Formula1Downloader.Downloaders;

namespace Formula1Downloader
{
	public partial class ViewController : NSViewController
	{
		public ViewController (IntPtr handle) : base (handle)
		{
		}

		private List<Video> _videos = new List<Video>();
		private readonly Queue<Tuple<Video, string>> _downloadQueue = new Queue<Tuple<Video, string>>();
		private readonly List<char> _invalidChars = new List<char>(Path.GetInvalidFileNameChars())
		{
			':',
			'\\'
		};
		private string _defaultSaveDirectory;
		private int _currentVideo = 0;
		private int _totalVideos = 0;

		private string DefaultSaveDirectory
		{
			get
			{
				if (_defaultSaveDirectory != null)
					return _defaultSaveDirectory;

				string userProfileFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
				string downloadsFolder = Path.Combine(userProfileFolder, "Downloads");
				return _defaultSaveDirectory = Directory.Exists(downloadsFolder) ? downloadsFolder : userProfileFolder;
			}
		}

		public override void PrepareForSegue(NSStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			if (segue.Identifier == "ChooseVideosSegue")
			{
				var dialog = segue.DestinationController as ChooseVideos;
				dialog.Videos = _videos;
				dialog.Presentor = this;
				dialog.DialogAccepted += (s, e) =>
				{
					_totalVideos = dialog.DataSource.Videos.Count(x => x.Selected);
					_currentVideo = 1;

					if (_totalVideos == 1)
					{
						HandleSingleVideo(dialog.DataSource.Videos.First(x => x.Selected));
					}
					else if (_totalVideos > 1)
					{
						var save = new NSOpenPanel
						{
							CanChooseDirectories = true,
							CanCreateDirectories = true,
							CanChooseFiles = false,
							DirectoryUrl = new NSUrl(DefaultSaveDirectory),
							Title = "Save Video File",
							Prompt = "Save downloaded videos here"
						};
						save.BeginSheet(View.Window, (result) =>
						{
							if (result == (int)NSPanelButtonType.Ok)
							{
								foreach (var v in dialog.DataSource.Videos)
								{
									if (v.Selected)
									{
										string filePath = Path.Combine(save.Url.Path, CleanFileName(v.Title)) + (PreferMP4.State.HasFlag(NSCellStateValue.On) ? ".mp4" : ".flv");
										AddToQueue(v, filePath);
									}
								}
								ProcessQueue();
							}
							else
							{
								ToggleUI(true);
							}
						});
					}
					else
					{
						ToggleUI(true);
					}
				};
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			URLTextField.BecomeFirstResponder();
		}

		partial void DownloadButtonClicked(NSObject sender)
		{
			// Otherwise the string disappears after we disable it.
			URLTextField.StringValue = URLTextField.StringValue;
			ToggleUI(false);

			if (URLTextField.StringValue.StartsWith("http://"))
				URLTextField.StringValue = URLTextField.StringValue.Replace("http://", "https://");
			if (URLTextField.StringValue.StartsWith("www.formula1.com") || URLTextField.StringValue.StartsWith("formula1.com"))
				URLTextField.StringValue = "https://" + URLTextField.StringValue;

			bool isURLvalid = Uri.TryCreate(URLTextField.StringValue, UriKind.Absolute, out Uri videoURI)
				&& videoURI.Scheme == Uri.UriSchemeHttps
				&& (videoURI.Host == "www.formula1.com" || videoURI.Host == "formula1.com");

			if (!isURLvalid)
			{
				new NSAlert()
				{
					AlertStyle = NSAlertStyle.Critical,
					InformativeText = "Please check if the URL is valid",
					MessageText = "Invalid URL"
				}.BeginSheet(View.Window, () => ToggleUI(true));

				return;
			}

			_videos = F1Utils.GetVideosFromUri(videoURI);

			if (_videos.Count == 1)
			{
				HandleSingleVideo(_videos.First());
			}
			else if (_videos.Count > 1)
			{
				PerformSegue("ChooseVideosSegue", this);
			}
			else
			{
				new NSAlert()
				{
					AlertStyle = NSAlertStyle.Warning,
					InformativeText = "Could not find any video in that URL",
					MessageText = "No videos found"
				}.BeginSheet(View.Window, () => ToggleUI(true));
			}
		}

		private void HandleSingleVideo(Video video)
		{
			var save = new NSSavePanel
			{
				DirectoryUrl = new NSUrl(DefaultSaveDirectory),
				AllowedFileTypes = new string[] {
					PreferMP4.State.HasFlag(NSCellStateValue.On) ? "mp4" : "flv"
				},
				Title = "Save Video File",
				NameFieldStringValue = CleanFileName(video.Title)
			};
			save.BeginSheet(View.Window, (result) =>
			{
				if (result == (int)NSPanelButtonType.Ok)
				{
					AddToQueue(video, save.Url.Path);
					ProcessQueue();
				}
				else
				{
					ToggleUI(true);
				}
			});
		}

		private void ToggleUI(bool enable)
		{
			DownloadButton.Enabled = enable;
			URLTextField.Enabled = enable;
			PreferMP4.Enabled = enable;
		}


		private void AddToQueue(Video video, string filePath)
		{
			_downloadQueue.Enqueue(Tuple.Create(video, filePath));
		}

		private bool ProcessQueue()
		{
			if (_downloadQueue.Count > 0)
			{
				var item = _downloadQueue.Dequeue();

				if (_totalVideos > 1)
				{
					VideoProgressLabel.StringValue = $"{_currentVideo}/{_totalVideos}";
					VideoProgressLabel.Hidden = false;
				}
				DownloadVideo(item.Item1, item.Item2);
				return true;
			}

			return false;
		}

		private void DownloadVideo(Video video, string filePath)
		{
			Downloader d;

			if (PreferMP4.State.HasFlag(NSCellStateValue.On))
				d = new MP4Downloader(video, filePath);
			else
				d = new FLVDownloader(video, filePath);

			d.ProgressChanged += OnProgressChanged;
			d.DownloadComplete += OnDownloadComplete;
			d.StartDownload();
		}

		private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			ProgressBar.DoubleValue = e.ProgressPercentage;
		}

		private void OnDownloadComplete(object sender, EventArgs e)
		{
			_currentVideo++;
			ProgressBar.DoubleValue = 0;
			if (!ProcessQueue())
			{
				VideoProgressLabel.Hidden = true;
				_totalVideos = 0;

				new NSAlert()
				{
					AlertStyle = NSAlertStyle.Informational,
					InformativeText = "Video(s) downloaded",
					MessageText = "Done"
				}.BeginSheet(View.Window, () => ToggleUI(true));
			}
		}

		private string CleanFileName(string fileName)
		{
			return _invalidChars.Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
		}
	}
}
