using AVFoundation;
using AVKit;
using Foundation;
using UIKit;
namespace Sample.iOS
{
	public class VideoSampleViewController : AVPlayerViewController
	{
	
		public VideoSampleViewController()
		{
			Title = "Lecteur vidéo";

			View.BackgroundColor = UIColor.White;
			this.EdgesForExtendedLayout = UIRectEdge.None;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            // Création de l'url de la vidéo
            NSUrl videoURL = NSUrl.FromFilename("video.m4v");

            // Chargement du lecteur
            Player = new AVPlayer(videoURL);
            ShowsPlaybackControls = true;

			Player.Play();
		}

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            Player.Pause();
        }
	}
}