using System;
using AppKit;
using CoreGraphics;

namespace Formula1Downloader
{
	public class VideoTableDelegate: NSTableViewDelegate
	{
		#region Private Variables
		private readonly VideoTableDataSource DataSource;
		#endregion

		#region Constructors
		public VideoTableDelegate (VideoTableDataSource datasource)
		{
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
							DataSource.Videos[(int) row].Selected = false;
							break;
						case NSCellStateValue.On:
							DataSource.Videos[(int)row].Selected = true;
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
				view.TextField.StringValue = DataSource.Videos[(int) row].Title;
				view.TextField.Tag = row;
				break;
			}

			return view;
		}
		#endregion
	}
}
