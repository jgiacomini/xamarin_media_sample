using System;
using UIKit;
namespace Sample.iOS
{
	public class VideoSampleViewController : UIViewController
	{
		public VideoSampleViewController()
		{
			Title = "Video sample";

			View.BackgroundColor = UIColor.White;
			this.EdgesForExtendedLayout = UIRectEdge.None;
		}	
	}
}
