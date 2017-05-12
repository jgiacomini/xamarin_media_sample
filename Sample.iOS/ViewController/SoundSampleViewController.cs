using System;
using System.IO;
using System.Threading.Tasks;
using AudioToolbox;
using AVFoundation;
using CoreGraphics;
using Foundation;
using Sample.iOS.Utils;
using UIKit;

namespace Sample.iOS
{
    public class SoundSampleViewController : UIViewController
	{
        #region Champs
        private UIButton _playButton;
		private UIButton _pauseButton;
		private UIButton _stopButton;
		private UISlider _volumeSlider;
		private UILabel _numberOfLoopsLabel;
		private UIStepper _numberOfLoopsSteppers;
		private UIButton _playSoundButton;
        private	UIButton _loadPlayerFromDataButton;
        #endregion

        public SoundSampleViewController()
		{
			Title = "Sound sample";

			View.BackgroundColor = UIColor.White;
			this.EdgesForExtendedLayout = UIRectEdge.None;

            AudioManager.Instance.LoadFromUrl("musique.mp3");

			_playButton = UIButton.FromType(UIButtonType.System);
			_playButton.SetTitle("Lecture", UIControlState.Normal);
			_playButton.TouchUpInside += _playButton_TouchUpInside;
			_playButton.Frame = new CGRect(10, 10, View.Bounds.Width - 20, 40);

			_pauseButton = UIButton.FromType(UIButtonType.System);
			_pauseButton.SetTitle("Pause", UIControlState.Normal);
			_pauseButton.TouchUpInside += _pauseButton_TouchUpInside;
			_pauseButton.Frame = new CGRect(10, 60, View.Bounds.Width - 20, 40);
            _pauseButton.Hidden = true;


			_stopButton = UIButton.FromType(UIButtonType.System);
			_stopButton.SetTitle("Stop", UIControlState.Normal);
			_stopButton.TouchUpInside += _stopButton_TouchUpInside;
			_stopButton.Frame = new CGRect(10, 110, View.Bounds.Width - 20, 40);
            _stopButton.Hidden =true;

			_volumeSlider = new UISlider();
			_volumeSlider.ValueChanged += _volumeSlider_ValueChanged;	
			_volumeSlider.Frame = new CGRect(10, 160, View.Bounds.Width - 20, 40);
            _volumeSlider.Value = AudioManager.Instance.Volume;

			_numberOfLoopsLabel = new UILabel();
			_numberOfLoopsLabel.Frame = new CGRect(10, 200, 80, 40);

			_numberOfLoopsSteppers = new UIStepper();
			_numberOfLoopsSteppers.MinimumValue = -1;
			_numberOfLoopsSteppers.ValueChanged += _numberOfLoopsSteppers_ValueChanged;
			_numberOfLoopsSteppers.Frame = new CGRect(View.Bounds.Width - 20 - 80, 200, 80, 40);
            _numberOfLoopsSteppers.Value = AudioManager.Instance.NumberOfLoops;

			_playSoundButton = UIButton.FromType(UIButtonType.System);
			_playSoundButton.SetTitle("Jouer un bruitage", UIControlState.Normal);
			_playSoundButton.TouchUpInside += _playSoundButton_TouchUpInside;
			_playSoundButton.Frame = new CGRect(10, 240, View.Bounds.Width - 20, 40);

			_loadPlayerFromDataButton = UIButton.FromType(UIButtonType.System);
			_loadPlayerFromDataButton.SetTitle("Charger musique depuis un tableau d'octets", UIControlState.Normal);
            _loadPlayerFromDataButton.TouchUpInside += _loadPlayerFromDataButton_TouchUpInside;
			_loadPlayerFromDataButton.Frame = new CGRect(10, 280, View.Bounds.Width - 20, 40);

			View.AddSubviews(_playButton, 
			                 _pauseButton,
			                 _stopButton,
			                 _volumeSlider,
			                 _numberOfLoopsSteppers,
			                 _numberOfLoopsLabel,
                             _playSoundButton,
                             _loadPlayerFromDataButton);
		}


		void _numberOfLoopsSteppers_ValueChanged(object sender, EventArgs e)
		{
			_numberOfLoopsLabel.Text = _numberOfLoopsSteppers.Value.ToString();
            AudioManager.Instance.NumberOfLoops = (nint)_numberOfLoopsSteppers.Value;
		}

		void _volumeSlider_ValueChanged(object sender, EventArgs e)
		{
            AudioManager.Instance.Volume = _volumeSlider.Value;
		}

		void _playButton_TouchUpInside(object sender, EventArgs e)
		{
			AudioManager.Instance.Play();
            _stopButton.Hidden = false;
			_pauseButton.Hidden = false;
			_playButton.Hidden = true;
		}

		void _pauseButton_TouchUpInside(object sender, EventArgs e)
		{
			AudioManager.Instance.Pause();
            _stopButton.Hidden = false;
			_pauseButton.Hidden = true;
			_playButton.Hidden = false;
		}

		void _stopButton_TouchUpInside(object sender, EventArgs e)
		{
			AudioManager.Instance.Stop();
            _stopButton.Hidden = true;
			_pauseButton.Hidden = true;
            _playButton.Hidden = false;
		}

		async void _playSoundButton_TouchUpInside(object sender, EventArgs e)
		{
			await TestSoundAsync();
		}

		void _loadPlayerFromDataButton_TouchUpInside(object sender, EventArgs e)
		{
			var bytes = File.ReadAllBytes("musique.mp3");
            AudioManager.Instance.LoadFromData(bytes);
		}

		public async Task TestSoundAsync()
		{
			NSUrl url = NSUrl.FromFilename("sound.mp3");
			SystemSound systemSound = new SystemSound(url);
			await systemSound.PlayAlertSoundAsync();
		}
	}
}
