using System;
using AppKit;
using CoreGraphics;
using Foundation;
using System.Collections;
using System.Collections.Generic;

namespace Formula1Downloader
{
	public class VideoTableDataSource : NSTableViewDataSource
	{
		#region Public Variables
		public List<Video> Videos = new List<Video>();
		public List<Video> SelectedVideos = new List<Video>();
		#endregion

		#region Constructors
		public VideoTableDataSource ()
		{
		}
		#endregion

		#region Override Methods
		public override nint GetRowCount (NSTableView tableView)
		{
			return Videos.Count;
		}
		#endregion
	}
}
