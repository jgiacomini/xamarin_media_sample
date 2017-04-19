using System;
using System.Threading.Tasks;
using AudioToolbox;
using AVFoundation;
using CoreGraphics;
using Foundation;
using UIKit;
namespace Sample.iOS
{
	public class SoundSampleViewController : UIViewController
	{
		private AVAudioPlayer _musicPlayer;


		private UIButton _playButton;
		private UIButton _pauseButton;
		private UIButton _stopButton;
		private UISlider _volumeSlider;
		private UILabel _numberOfLoopsLabel;
		private UIStepper _numberOfLoopsSteppers;

		private float _defaultVolume = 1.0f;

		public SoundSampleViewController()
		{
			Title = "Sound sample";

			View.BackgroundColor = UIColor.White;
			this.EdgesForExtendedLayout = UIRectEdge.None;
			TestMusic();

			// Initialize Audio
			var session = AVAudioSession.SharedInstance();
			session.SetCategory(AVAudioSessionCategory.Ambient);
			session.SetActive(true);

			_playButton = UIButton.FromType(UIButtonType.System);
			_playButton.SetTitle("Lecture", UIControlState.Normal);
			_playButton.TouchUpInside += _playButton_TouchUpInside;
			_playButton.Frame = new CGRect(10, 10, View.Bounds.Width - 20, 40);

			_pauseButton = UIButton.FromType(UIButtonType.System);
			_pauseButton.SetTitle("Pause", UIControlState.Normal);
			_pauseButton.TouchUpInside += _pauseButton_TouchUpInside;;
			_pauseButton.Frame = new CGRect(10, 60, View.Bounds.Width - 20, 40);


			_stopButton = UIButton.FromType(UIButtonType.System);
			_stopButton.SetTitle("Stop", UIControlState.Normal);
			_stopButton.TouchUpInside += _stopButton_TouchUpInside;
			_stopButton.Frame = new CGRect(10, 110, View.Bounds.Width - 20, 40);

			_volumeSlider = new UISlider();
			_volumeSlider.ValueChanged += _volumeSlider_ValueChanged;	
			_volumeSlider.Frame = new CGRect(10, 160, View.Bounds.Width - 20, 40);
			_volumeSlider.Value = _defaultVolume;

			_numberOfLoopsLabel = new UILabel();
			_numberOfLoopsLabel.Frame = new CGRect(10, 200, 80, 40);

			_numberOfLoopsSteppers = new UIStepper();
			_numberOfLoopsSteppers.MinimumValue = -1;
			_numberOfLoopsSteppers.ValueChanged += _numberOfLoopsSteppers_ValueChanged;
			_numberOfLoopsSteppers.Frame = new CGRect(View.Bounds.Width - 20 - 80, 200, 80, 40);

			View.AddSubviews(_playButton, 
			                 _pauseButton,
			                 _stopButton,
			                 _volumeSlider,
			                 _numberOfLoopsSteppers,
			                _numberOfLoopsLabel);
		}

		void _volumeSlider_ValueChanged(object sender, EventArgs e)
		{
			_musicPlayer.Volume = _volumeSlider.Value;
		}

		void _playButton_TouchUpInside(object sender, EventArgs e)
		{
			_musicPlayer.Play();
		}

		void _pauseButton_TouchUpInside(object sender, EventArgs e)
		{
			_musicPlayer.Pause();
		}

		void _stopButton_TouchUpInside(object sender, EventArgs e)
		{
			_musicPlayer.Stop();
			TestSoundAsync();
		}

		void _numberOfLoopsSteppers_ValueChanged(object sender, EventArgs e)
		{
			_numberOfLoopsLabel.Text = _numberOfLoopsSteppers.Value.ToString();
			_musicPlayer.NumberOfLoops = (nint)_numberOfLoopsSteppers.Value;
		}

		void TestMusic()
		{
			NSUrl musicURL = NSUrl.FromString("musique.mp3");
			_musicPlayer = AVAudioPlayer.FromUrl(musicURL);
			_musicPlayer.Volume = _defaultVolume;
			_musicPlayer.FinishedPlaying += delegate
			{	
				_musicPlayer.Dispose();
				_musicPlayer = null;
			};
			_musicPlayer.NumberOfLoops = -1;
		}

		async Task TestSoundAsync()
		{
			NSUrl url = NSUrl.FromFilename ("sound.mp3");
			SystemSound systemSound = new SystemSound(url);
			await systemSound.PlayAlertSoundAsync ();
		}
	}
}
