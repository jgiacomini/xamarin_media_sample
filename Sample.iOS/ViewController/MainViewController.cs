using System;
using CoreGraphics;
using UIKit;

namespace Sample.iOS
{
	public class MainViewController : UIViewController
	{

		private UIButton _loadImageButton;
		private UIButton _soundButton;
		private UIButton _videoButton;


		public MainViewController()
		{
			Title = "Menu";
			View.BackgroundColor = UIColor.White;
			this.EdgesForExtendedLayout = UIRectEdge.None;
		}
		public override void ViewDidLoad()
		{
			_loadImageButton = UIButton.FromType(UIButtonType.System);
			_loadImageButton.SetTitle("Charger et sauvegarder une image", UIControlState.Normal);
			_loadImageButton.TouchUpInside += _imageButton_TouchUpInside;
			_loadImageButton.Frame = new CGRect(10, 10, View.Bounds.Width, 40);

			_soundButton = UIButton.FromType(UIButtonType.System);
			_soundButton.SetTitle("Son", UIControlState.Normal);
			_soundButton.TouchUpInside += _soundButton_TouchUpInside;
			_soundButton.Frame = new CGRect(10, 60, View.Bounds.Width, 40);


			_videoButton = UIButton.FromType(UIButtonType.System);
			_videoButton.SetTitle("Vidéo", UIControlState.Normal);
			_videoButton.TouchUpInside += _videoButton_TouchUpInside;
			_videoButton.Frame = new CGRect(10, 110, View.Bounds.Width, 40);


			View.AddSubviews(_loadImageButton, _soundButton, _videoButton);
		}

		void _imageButton_TouchUpInside(object sender, EventArgs e)
		{
			this.NavigationController.PushViewController(new ImageSampleViewController(), true);
		}

		void _soundButton_TouchUpInside(object sender, EventArgs e)
		{
			this.NavigationController.PushViewController(new SoundSampleViewController(), true);
		}

		void _videoButton_TouchUpInside(object sender, EventArgs e)
		{
			this.NavigationController.PushViewController(new VideoSampleViewController(), true);
		}
	}
}
