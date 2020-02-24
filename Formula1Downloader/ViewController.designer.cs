// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Formula1Downloader
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSButton DownloadButton { get; set; }

		[Outlet]
		AppKit.NSButton PreferMP4 { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator ProgressBar { get; set; }

		[Outlet]
		AppKit.NSTextField URLTextField { get; set; }

		[Outlet]
		AppKit.NSTextField VideoProgressLabel { get; set; }

		[Action ("DownloadButtonClicked:")]
		partial void DownloadButtonClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (DownloadButton != null) {
				DownloadButton.Dispose ();
				DownloadButton = null;
			}

			if (PreferMP4 != null) {
				PreferMP4.Dispose ();
				PreferMP4 = null;
			}

			if (ProgressBar != null) {
				ProgressBar.Dispose ();
				ProgressBar = null;
			}

			if (URLTextField != null) {
				URLTextField.Dispose ();
				URLTextField = null;
			}

			if (VideoProgressLabel != null) {
				VideoProgressLabel.Dispose ();
				VideoProgressLabel = null;
			}
		}
	}
}
