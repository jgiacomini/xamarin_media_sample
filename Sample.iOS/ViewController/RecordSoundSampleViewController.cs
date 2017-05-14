﻿using System;
using System.Diagnostics;
using System.IO;
using AudioToolbox;
using AVFoundation;
using CoreGraphics;
using Foundation;
using Sample.iOS.Utils;
using UIKit;

namespace Sample.iOS
{
    public class RecordSoundSampleViewController : UIViewController
    {

        #region Champs
        private AVAudioRecorder _recorder;
        private Stopwatch _stopwatch = null;
        private NSUrl _audioFilePath = null;
        private UILabel _stateOfRecordingLabel;
        private UIButton _recordButton;
        private UIButton _stopRecordButton;
        private UIButton _playSoundButton;
        #endregion

        public RecordSoundSampleViewController()
        {
            Title = "Enregistrer un son";

            View.BackgroundColor = UIColor.White;
            this.EdgesForExtendedLayout = UIRectEdge.None;

            // Initialise la session audio
            AudioSession.Initialize();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _stateOfRecordingLabel = new UILabel();
            _stateOfRecordingLabel.Frame = new CGRect(10, 60, View.Bounds.Width - 20, 40);
            _stateOfRecordingLabel.TextAlignment = UITextAlignment.Center;

            _recordButton = UIButton.FromType(UIButtonType.System);
            _recordButton.Frame = new CGRect(10, 110, View.Bounds.Width - 20, 40);
            _recordButton.SetTitle("Lancer l'enregistrement", UIControlState.Normal);

            _stopRecordButton = UIButton.FromType(UIButtonType.System);
            _stopRecordButton.Frame = new CGRect(10, 160, View.Bounds.Width - 20, 40);
            _stopRecordButton.SetTitle("Arreter l'enregistrement", UIControlState.Normal);
            _stopRecordButton.Enabled = false;

            _playSoundButton = UIButton.FromType(UIButtonType.System);
            _playSoundButton.Frame = new CGRect(10, 210, View.Bounds.Width - 20, 40);
            _playSoundButton.SetTitle("Ecouter l'enregistrement", UIControlState.Normal);
            _playSoundButton.Enabled = false;

            _recordButton.TouchUpInside += _recordButton_TouchUpInside;

            _stopRecordButton.TouchUpInside += _stopRecordButton_TouchUpInside;

            _playSoundButton.TouchUpInside += _playSoundButton_TouchUpInside;

            View.AddSubviews(_stateOfRecordingLabel,
                                 _recordButton,
                                  _stopRecordButton,
                                 _playSoundButton);
        }

        private void _recordButton_TouchUpInside(object sender, EventArgs e)
        {
            _stateOfRecordingLabel.Text = "Début de l'enregistrement";

            if (!PrepareAudioRecording())
            {
                _stateOfRecordingLabel.Text = "Impossible de lancer l'enregistrement";

                return;
            }

            if (!_recorder.Record())
            {
                _stateOfRecordingLabel.Text = "Impossible de lancer l'enregistrement";

                return;
            }

            _stopwatch = new Stopwatch();
            _stopwatch.Start();
            _stateOfRecordingLabel.Text = "Enregistrement";
            _recordButton.Enabled = false;
            _stopRecordButton.Enabled = true;
            _playSoundButton.Enabled = false;
        }

        private bool PrepareAudioRecording()
        {
            var audioSession = AVAudioSession.SharedInstance();
            var err = audioSession.SetCategory(AVAudioSessionCategory.Record);
            if (err != null)
            {
                Console.WriteLine("[audioSession]Impossible de définir la catégorie PlayAndRecord : {0}", err);
                return false;
            }
            err = audioSession.SetActive(true);
            if (err != null)
            {
                Console.WriteLine("[audioSession]Impossible de rendre actif la session audio: {0}", err);
                return false;
            }

            string fileName = string.Format("MonEnregistrement_{0}.aac", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string fileFullPath = Path.Combine(Path.GetTempPath(), fileName);

            // Convertit le fileFullPath en NSUrl
            _audioFilePath = NSUrl.FromFilename(fileFullPath);
                       
            // Crée le profil d'enregistrement
            var audioSettings = new AudioSettings();
            audioSettings.Format = AudioFormatType.MPEG4AAC;
            audioSettings.AudioQuality = AVAudioQuality.Max;

            // Création du dictaphone
            NSError error = null;

            _recorder = AVAudioRecorder.Create(_audioFilePath, audioSettings, out error);
            if (_recorder == null || error != null)
            {
                Console.WriteLine(string.Format("Erreur lors de la création du dictaphone : {0}",error));
                return false;
            }

            // Prépare l'enregistreur à enregister le son
            // Cette étape n'est pas obligatoire, mais permet de lancer l'enregistrement plus vite car elle pré-crée le fichier audio et pépare le système à l'enregistrement
            if (!_recorder.PrepareToRecord())
            {
                _recorder.Dispose();
                _recorder = null;
                return false;
            }

            return true;
        }

        private void _stopRecordButton_TouchUpInside(object sender, EventArgs arg)
        {
            _recorder.Stop();
            _recorder.Dispose();
            _recorder = null;

            _stateOfRecordingLabel.Text = string.Format("Durée de l'enregistrement : {0:hh\\:mm\\:ss}", this._stopwatch.Elapsed);
            _stopwatch.Stop();
            _recordButton.Enabled = true;
            _stopRecordButton.Enabled = false;
            _playSoundButton.Enabled = true;
        }


        private async void _playSoundButton_TouchUpInside(object sender, EventArgs arg)
        {
            _playSoundButton.Enabled = false;

            try
            {
                Console.WriteLine("Ecoute de l'enregistrement :" + this._audioFilePath.ToString());
                _stateOfRecordingLabel.Text = "Ecoute de l'enregistrement";

                AudioSession.Category = AudioSessionCategory.AmbientSound;
                AudioSession.SetActive(true);

                AudioManager.Instance.LoadFromUrl(this._audioFilePath.ToString());
                AudioManager.Instance.NumberOfLoops = 0;
                AudioManager.Instance.Volume = 1;
                await AudioManager.Instance.PlayAsync();
                _playSoundButton.Enabled = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Impossible de jouer l'enregistrement audio");
                Console.WriteLine(ex.Message);
                _playSoundButton.Enabled = true;
            }
        }
    }
}