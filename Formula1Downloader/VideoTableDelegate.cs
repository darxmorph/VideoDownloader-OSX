using System;
using System.Linq;
using AppKit;
using CoreGraphics;
using Foundation;
using System.Collections;
using System.Collections.Generic;

namespace Formula1Downloader
{
	public class VideoTableDelegate: NSTableViewDelegate
	{
		#region Constants 
		private const string CellIdentifier = "ProdCell";
		#endregion

		#region Private Variables
		private VideoTableDataSource DataSource;
		// private ViewController Controller;
		#endregion

		#region Constructors
		public VideoTableDelegate (VideoTableDataSource datasource)
		{
			// this.Controller = controller;
			this.DataSource = datasource;
		}
		#endregion

		public override bool SelectionShouldChange (NSTableView tableView)
		{
			return false;
		}

		private void ConfigureTextField (NSTableCellView view, nint row)
		{
			// Add to view
			view.TextField.AutoresizingMask = NSViewResizingMask.WidthSizable;
			view.AddSubview (view.TextField);

			// Configure
			view.TextField.BackgroundColor = NSColor.Clear;
			view.TextField.Bordered = false;
			view.TextField.Selectable = false;

			// Tag view
			view.TextField.Tag = row;
		}


		#region Override Methods
		public override NSView GetViewForItem (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			// This pattern allows you reuse existing views when they are no-longer in use.
			// If the returned view is null, you instance up a new view
			// If a non-null view is returned, you modify it enough to reflect the new data
			NSTableCellView view = (NSTableCellView)tableView.MakeView (tableColumn.Title, this);
			if (view == null) {
				view = new NSTableCellView ();

				// Configure the view
				view.Identifier = tableColumn.Title;

				// Take action based on title
				switch (tableColumn.Title) {
				case "Name":
					view.TextField = new NSTextField (new CGRect (0, 0, 400, 16));
					ConfigureTextField (view, row);
					break;
				case "Action":
					// Create new button
					var button = new NSButton (new CGRect (0, 0, 81, 16));
					button.SetButtonType (NSButtonType.Switch);
					button.Title = "Download";
					button.Tag = row;

					// Wireup events
					button.Activated += (sender, e) => {
						var btw = sender as NSButton;

						switch (btw.State) {
						case NSCellStateValue.Off:
							DataSource.SelectedVideos.Remove(DataSource.Videos[(int) row]);
							break;
						case NSCellStateValue.On:
							DataSource.SelectedVideos.Add(DataSource.Videos[(int) row]);
							break;
						}
					};

					// Add to view
					view.AddSubview (button);
					break;
				}
			}

			// Setup view based on the column selected
			switch (tableColumn.Title) {
			case "Name":
				// view.TextField.StringValue = DataSource.Videos[(int) row];
				view.TextField.StringValue = DataSource.Videos[(int) row].Title;
				view.TextField.Tag = row;
				break;
			/*case "Action":
				foreach (NSView subview in view.Subviews) {
					var btw = subview as NSButton;
					if (btw != null) {
						btn.Tag = row;
					}
				}
				break;*/
			}

			return view;
		}
		#endregion
	}
}
