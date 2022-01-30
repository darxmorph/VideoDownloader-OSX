using AppKit;
using Foundation;

namespace Formula1Downloader
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : NSApplicationDelegate
	{
		public AppDelegate ()
		{
		}

		public override void DidFinishLaunching (NSNotification notification)
		{
			// Insert code here to initialize your application
		}

		public override void WillTerminate (NSNotification notification)
		{
			// Insert code here to tear down your application
		}

		public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
		{
			return true;
		}

		[Action("redditPost:")]
		public void redditPost (NSObject sender) {
			System.Diagnostics.Process.Start ("https://www.reddit.com/r/formula1/comments/4od4it/i_made_a_downloader_for_formula1com_videos_let_me/");
		}

		[Action("projectGitHub:")]
		public void projectGitHub (NSObject sender) {
			System.Diagnostics.Process.Start ("https://github.com/darxmorph/VideoDownloader-OSX");
		}
	}
}

