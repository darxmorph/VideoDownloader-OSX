// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Formula1Downloader
{
	[Register ("ChooseVideos")]
	partial class ChooseVideos
	{
		[Outlet]
		AppKit.NSTableColumn VideoActionColumn { get; set; }

		[Outlet]
		AppKit.NSTableColumn VideoNameColumn { get; set; }

		[Outlet]
		AppKit.NSTableView VideosTable { get; set; }

		[Action ("AcceptSheet:")]
		partial void AcceptSheet (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (VideoActionColumn != null) {
				VideoActionColumn.Dispose ();
				VideoActionColumn = null;
			}

			if (VideoNameColumn != null) {
				VideoNameColumn.Dispose ();
				VideoNameColumn = null;
			}

			if (VideosTable != null) {
				VideosTable.Dispose ();
				VideosTable = null;
			}
		}
	}
}
