using System;
using AppKit;
using System.Collections.Generic;

namespace Formula1Downloader
{
	public class VideoTableDataSource : NSTableViewDataSource
	{
		#region Public Variables
		public List<Video> Videos { get; }
		#endregion

		#region Constructors
		public VideoTableDataSource (List<Video> videos)
		{
			Videos = videos;
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
