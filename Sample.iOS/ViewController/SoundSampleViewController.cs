using System;
using System.Threading.Tasks;
using AudioToolbox;
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
		private UILabel _numberOfLoopLegendLabel;
		private UILabel _numberOfLoopsLabel;
		private UIStepper _numberOfLoopsSteppers;
		private UIButton _playSoundButton;
        #endregion

        public SoundSampleViewController()
		{
			Title = "Lecteur audio";

			View.BackgroundColor = UIColor.White;
			this.EdgesForExtendedLayout = UIRectEdge.None;

            // On charge la musique
            AudioManager.Instance.FinishedPlaying += FinishedPlaying;
            AudioManager.Instance.LoadFromUrl("musique.mp3");

			_playButton = UIButton.FromType(UIButtonType.System);
			_playButton.SetTitle("Lecture", UIControlState.Normal);
			_playButton.TouchUpInside += _playButton_TouchUpInside;
			_playButton.Frame = new CGRect(10, 10, View.Bounds.Width - 20, 40);

			_pauseButton = UIButton.FromType(UIButtonType.System);
			_pauseButton.SetTitle("Pause", UIControlState.Normal);
			_pauseButton.TouchUpInside += _pauseButton_TouchUpInside;
			_pauseButton.Frame = new CGRect(10, 60, View.Bounds.Width - 20, 40);
            _pauseButton.Enabled = false;


			_stopButton = UIButton.FromType(UIButtonType.System);
			_stopButton.SetTitle("Arrêt", UIControlState.Normal);
			_stopButton.TouchUpInside += _stopButton_TouchUpInside;
			_stopButton.Frame = new CGRect(10, 110, View.Bounds.Width - 20, 40);
            _stopButton.Enabled = false;

			_volumeSlider = new UISlider();
			_volumeSlider.ValueChanged += _volumeSlider_ValueChanged;	
			_volumeSlider.Frame = new CGRect(10, 160, View.Bounds.Width - 20, 40);
            _volumeSlider.Value = AudioManager.Instance.Volume;

			_numberOfLoopLegendLabel = new UILabel();
			_numberOfLoopLegendLabel.Frame = new CGRect(10, 200, View.Bounds.Width - 20, 40);
			_numberOfLoopLegendLabel.Text = "Nombre de boucles :";

			_numberOfLoopsLabel = new UILabel();
			_numberOfLoopsLabel.Frame = new CGRect(10, 240, 80, 40);
            _numberOfLoopsLabel.Text = AudioManager.Instance.NumberOfLoops.ToString();

			_numberOfLoopsSteppers = new UIStepper();
			_numberOfLoopsSteppers.MinimumValue = -1;
			_numberOfLoopsSteppers.ValueChanged += _numberOfLoopsSteppers_ValueChanged;
			_numberOfLoopsSteppers.Frame = new CGRect(View.Bounds.Width - 20 - 80, 240, 80, 40);
            _numberOfLoopsSteppers.Value = AudioManager.Instance.NumberOfLoops;


			_playSoundButton = UIButton.FromType(UIButtonType.System);
			_playSoundButton.SetTitle("Jouer un bruitage", UIControlState.Normal);
			_playSoundButton.TouchUpInside += _playSoundButton_TouchUpInside;
			_playSoundButton.Frame = new CGRect(10, 280, View.Bounds.Width - 20, 40);

			View.AddSubviews(_playButton, 
			                 _pauseButton,
			                 _stopButton,
			                 _volumeSlider,
                             _numberOfLoopLegendLabel,
                             _numberOfLoopsLabel,
			                 _numberOfLoopsSteppers,
                             _playSoundButton);
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

			//On désactive le bouton "Lecture" et on active "Arrêt" et "Pause"
			_stopButton.Enabled = true;
			_pauseButton.Enabled = true;
            _playButton.Enabled = false;
		}

		void _pauseButton_TouchUpInside(object sender, EventArgs e)
		{
			AudioManager.Instance.Pause();

            //On désactive le bouton "Pause" et on active "Arrêt" et "Pause"
            _pauseButton.Enabled = false;
			_stopButton.Enabled = true;
			_playButton.Enabled = true;
		}

		void _stopButton_TouchUpInside(object sender, EventArgs e)
		{
			//On désactive les boutons "Arrêt" et "Pause" et on active "Lecture"
			AudioManager.Instance.Stop();
            _stopButton.Enabled = false;
            _pauseButton.Enabled = false;
            _playButton.Enabled = true;
		}

		async void _playSoundButton_TouchUpInside(object sender, EventArgs e)
		{
			await TestSoundAsync();
		}

		public async Task TestSoundAsync()
		{
			NSUrl url = NSUrl.FromFilename("sound.mp3");
			SystemSound systemSound = new SystemSound(url);
			await systemSound.PlayAlertSoundAsync();
		}


        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            //Quand le viewController n'est plus affiché on stoppe la musique
            AudioManager.Instance.StopAndDispose();
            FinishedPlaying();
            AudioManager.Instance.FinishedPlaying -= FinishedPlaying;
        }

        void FinishedPlaying()
        {
            // Quand la musique est terminée repasse à l'état "arret"
            _stopButton_TouchUpInside(null, EventArgs.Empty);
        }
	}
}
