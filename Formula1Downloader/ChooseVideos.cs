// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Collections.Generic;
using Foundation;
using AppKit;

namespace Formula1Downloader
{
	public partial class ChooseVideos : NSViewController
	{
		public ChooseVideos (IntPtr handle) : base (handle)
		{
			
		}

		public override CoreGraphics.CGSize PreferredContentSize {
			get {
				return new CoreGraphics.CGSize(this.View.Frame.Size.Width, this.View.Frame.Size.Height);
			}
			set {
				// base.PreferredContentSize = value;
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		}

		#region Private Variables
		private List<Video> _VideoList;
		private NSViewController _presentor;
		#endregion

		#region Computed Properties
		public List<Video> Videos {
			get { return _VideoList; }
			set { _VideoList = value; }
		}

		public NSViewController Presentor {
			get { return _presentor; }
			set { _presentor = value; }
		}

		public VideoTableDataSource DataSource = new VideoTableDataSource ();

		private VideoTableDelegate _VideoTableDelegate { get;set; }
		#endregion

		#region Override Methods
		public override void ViewWillAppear ()
		{
			base.ViewWillAppear ();

			DataSource.Videos = _VideoList;

			// Populate the Product Table
			VideosTable.DataSource = DataSource;
			VideosTable.Delegate = new VideoTableDelegate (DataSource);
		}
		#endregion

		#region Private Methods
		private void CloseDialog() {
			Presentor.DismissViewController (this);
		}
		#endregion

		#region Custom Actions
		partial void AcceptSheet (Foundation.NSObject sender) {
			RaiseDialogAccepted();
			CloseDialog();
		}
		#endregion

		#region Events
		public EventHandler DialogAccepted;

		internal void RaiseDialogAccepted() {
			if (this.DialogAccepted != null)
				this.DialogAccepted (this, EventArgs.Empty);
		}
		#endregion
	}
}