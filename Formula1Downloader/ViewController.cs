using System;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using AppKit;
using Foundation;

using hdsdump;

namespace Formula1Downloader
{
	public partial class ViewController : NSViewController
	{
		public ViewController (IntPtr handle) : base (handle)
		{
		}

		private List<Video> _vids = new List<Video>();

		public override void PrepareForSegue (NSStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			// Take action based on the segue name
			switch (segue.Identifier) {
			case "ChooseVideosSegue":
				var dialog = segue.DestinationController as ChooseVideos;
				// dialog.VideoTitlesList = _vids;
				// dialog.DataSource.Videos = _vids;
				dialog.Videos = _vids;
				dialog.DialogAccepted += (s, e) => {
					if (dialog.DataSource.SelectedVideos.Count > 0) {
						bool notChosen = false;
						BackgroundWorker notMainThread = new BackgroundWorker ();
						notMainThread.DoWork += delegate {
							// string dir = ShowChooseDirectory(sender);
							// System.Threading.Thread.Sleep(2000);
							string dir = ShowChooseDirectory (sender);
							if (dir == "") {
								notChosen = true;
								return;
							}
								
							foreach (Video vid in dialog.DataSource.SelectedVideos) {
								string saveFilePath = Path.Combine (dir, CleanFileName (vid.Title)) + ".flv";
								F1Utils.getVideoUsingAdobeHDS (vid, saveFilePath, ProgressBar);
							}
						};
						notMainThread.RunWorkerCompleted += delegate {
							ProgressBar.DoubleValue = 0;
							DownloadButton.Enabled = true;
							URLTextField.Enabled = true;

							if (notChosen) {
								var notChosenAlert = new NSAlert () {
									AlertStyle = NSAlertStyle.Critical,
									InformativeText = "Please choose where to save",
									MessageText = "Error",
								};
								notChosenAlert.BeginSheet(NSApplication.SharedApplication.KeyWindow);
							} else {
								var videoDownloaded = new NSAlert () {
									AlertStyle = NSAlertStyle.Informational,
									InformativeText = "Video(s) downloaded",
									MessageText = "Done",
								};
								videoDownloaded.BeginSheet(NSApplication.SharedApplication.KeyWindow);
							}
						};
						notMainThread.RunWorkerAsync ();
					} else {
						ProgressBar.DoubleValue = 0;
						DownloadButton.Enabled = true;
						URLTextField.Enabled = true;
					}
					dialog.Presentor = this;
				};
				break;
			};
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			URLTextField.BecomeFirstResponder();
		}

		public override NSObject RepresentedObject {
			get {
				return base.RepresentedObject;
			}
			set {
				base.RepresentedObject = value;
			}
		}

		partial void DownloadButtonClicked(NSObject sender) {
			/*
			 * Some vids for testing
			https://www.formula1.com/content/fom-website/en/video/2016/4/Director's_Cut__Bahrain_2016.html
			https://www.formula1.com/content/fom-website/en/latest/features/2016/3/f1-2016-best-onboard-videos-australia.html
			https://www.formula1.com/content/fom-website/en/latest/features/2016/4/say-what--the-best-of-team-radio-from-bahrain.html
			https://www.formula1.com/content/fom-website/en/latest/features/2016/4/f1-best-radio-china-say-what.html
			*/

			DownloadButton.Enabled = false;
			URLTextField.StringValue = URLTextField.StringValue; // Otherwise the string disappears after we disable it. I don't know if there's a better way to achieve this...
			URLTextField.Enabled = false;

			Uri videoURI = null;

			if (!(URLTextField.StringValue.StartsWith("http://") || URLTextField.StringValue.StartsWith("https://")))
				URLTextField.StringValue = "https://" + URLTextField.StringValue;

			bool isURLvalid = (URLTextField.StringValue.StartsWith("http://www.formula1.com") || URLTextField.StringValue.StartsWith("https://www.formula1.com"))
				&& Uri.TryCreate(URLTextField.StringValue, UriKind.Absolute, out videoURI)
				&& (videoURI.Scheme == Uri.UriSchemeHttp || videoURI.Scheme == Uri.UriSchemeHttps);

			if (isURLvalid)
			{
				F1Utils.F1VideoTypes? videoType = F1Utils.getF1UriVideoType(videoURI);

				switch (videoType) {
				case F1Utils.F1VideoTypes.SingleVideo:
					Video vid = F1Utils.getF4MManifestURLsFromVideoURI(videoURI).First();

					string outFile = "";

					BackgroundWorker saveVideoToFile = new BackgroundWorker();

					saveVideoToFile.DoWork += delegate {
						outFile = ShowSaveAs(sender, vid.Title);
					};

					saveVideoToFile.RunWorkerCompleted += delegate {
						if (outFile == "") {
							var chooseWhereToSave = new NSAlert () {
								AlertStyle = NSAlertStyle.Critical,
								InformativeText = "Please choose where to save",
								MessageText = "Error",
							};
							chooseWhereToSave.BeginSheet(NSApplication.SharedApplication.KeyWindow);

							DownloadButton.Enabled = true;
							URLTextField.Enabled = true;

							return;
						}

						BackgroundWorker downloadWorker = new BackgroundWorker();
						downloadWorker.DoWork += delegate {
							F1Utils.getVideoUsingAdobeHDS(vid, outFile, ProgressBar);
						};
						downloadWorker.RunWorkerCompleted += delegate {
							ProgressBar.DoubleValue = 0;
							DownloadButton.Enabled = true;
							URLTextField.Enabled = true;

							var videoDownloaded = new NSAlert () {
								AlertStyle = NSAlertStyle.Informational,
								InformativeText = "Video downloaded",
								MessageText = "Done",
							};
							videoDownloaded.BeginSheet(NSApplication.SharedApplication.KeyWindow);
						};
						downloadWorker.RunWorkerAsync();
					};

					saveVideoToFile.RunWorkerAsync();
					break;

				case F1Utils.F1VideoTypes.H5AndVideo:
					Video[] videosToDownload = null;
					BackgroundWorker getManifests = new BackgroundWorker();
					getManifests.DoWork += delegate {
						videosToDownload = F1Utils.getF4MManifestURLsFromVideoURI(videoURI);
					};
					getManifests.RunWorkerCompleted += delegate {
						if (videosToDownload == null || videosToDownload.Length < 1) {
							DownloadButton.Enabled = true;
							URLTextField.Enabled = true;
							return;
						}

						_vids = videosToDownload.OfType<Video>().ToList();


						this.PerformSegue("ChooseVideosSegue", this);
					};
					getManifests.RunWorkerAsync();
					break;

				default:
					var unknownVidType = new NSAlert () {
						AlertStyle = NSAlertStyle.Critical,
						InformativeText = "Error obtaning video info from URL",
						MessageText = "Error",
					};
					unknownVidType.BeginSheet(NSApplication.SharedApplication.KeyWindow);
					DownloadButton.Enabled = true;
					URLTextField.Enabled = true;
					break;
				}
			}
			else {
				var invalidURLalert = new NSAlert () {
					AlertStyle = NSAlertStyle.Critical,
					InformativeText = "Please check if the URL is valid",
					MessageText = "Invalid URL",
				};
				invalidURLalert.BeginSheet(NSApplication.SharedApplication.KeyWindow);

				DownloadButton.Enabled = true;
				URLTextField.Enabled = true;
			}
		}

		string ShowSaveAs (NSObject sender, string DefaultFileName)
		{
			string ret = "";

			System.Threading.AutoResetEvent saveEvent = new System.Threading.AutoResetEvent(false);

			this.InvokeOnMainThread(new Action(() => {
				var save = new NSSavePanel ();
				save.DirectoryUrl = new NSUrl (Environment.GetFolderPath (Environment.SpecialFolder.UserProfile));
				save.AllowedFileTypes = new string[] { "flv" };
				save.Title = "Save Video File";
				save.NameFieldStringValue = CleanFileName(DefaultFileName);

				save.BeginSheet (NSApplication.SharedApplication.KeyWindow, (result) => {
					if (result == (int)NSPanelButtonType.Ok) {
						ret = save.Url.Path;
					}

					saveEvent.Set();
				});
			}));

			saveEvent.WaitOne ();

			return ret;
		}


		string ShowChooseDirectory (NSObject sender)
		{
			string ret = "";

			System.Threading.AutoResetEvent saveEvent = new System.Threading.AutoResetEvent(false);

			this.InvokeOnMainThread(new Action(() => {
				var save = new NSOpenPanel ();
				save.CanChooseDirectories = true;
				save.CanCreateDirectories = true;
				save.CanChooseFiles = false;
				save.DirectoryUrl = new NSUrl (Environment.GetFolderPath (Environment.SpecialFolder.UserProfile));
				save.AllowedFileTypes = new string[] { "flv" };
				save.Title = "Save Video File";
				save.Prompt = "Save downloaded videos here";
				save.BeginSheet (NSApplication.SharedApplication.KeyWindow, (result) => {
					if (result == (int)NSPanelButtonType.Ok) {
						ret = save.Url.Path;
					}

					saveEvent.Set();
				});
			}));

			saveEvent.WaitOne ();

			return ret;
		}

		private static string CleanFileName(string fileName)
		{
			return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
		}
	}
}
