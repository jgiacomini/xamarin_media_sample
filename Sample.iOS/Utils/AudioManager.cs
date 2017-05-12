using System;
using AVFoundation;
using Foundation;
using UIKit;

namespace Sample.iOS.Utils
{
	public class AudioManager
	{
        #region Champs statiques
        private static AudioManager _audioManager;
		#endregion

		#region Champs
		private AVAudioPlayer _musicPlayer;
        private string _currentUrl;
        private float _currentVolume = 1.0f;
        private nint _currentNumbersOfLoops = -1;
        #endregion

        private AudioManager()
		{
		}

        public event Action FinishedPlaying;

		public static AudioManager Instance
		{
			get
			{
				if (_audioManager == null)
				{
					_audioManager = new AudioManager();
				}

				return _audioManager;
			}
		}

		public float Volume
		{
			get
			{
				if (_musicPlayer == null)
				{
					return _currentVolume;
				}

				return _musicPlayer.Volume;
			}
			set
			{
                _currentVolume = value;
                if (_musicPlayer != null)
                {
                    _musicPlayer.Volume = _currentVolume;
                }
			}
		}

		public nint NumberOfLoops
		{
			get
			{
                if(_musicPlayer == null)
                {
                    return _currentNumbersOfLoops;
                }

                return _musicPlayer.NumberOfLoops;
			}
			set
			{
                _currentNumbersOfLoops = value;
                if (_musicPlayer != null)
                {
                    _musicPlayer.NumberOfLoops = _currentNumbersOfLoops;
                }
			}
		}

		public void Play()
		{
            // Si le player n'est pas créé
            if (_musicPlayer == null)
            {
                // Si une URL est définie on charge la musique
                if (_currentUrl != null)
                {
                    LoadFromUrl(_currentUrl);
                }
            }

			_musicPlayer?.Play();
		}

		public void Pause()
		{
			_musicPlayer?.Pause();
		}

		public void Stop()
		{
            if (_musicPlayer != null)
            {
                _musicPlayer.Stop();
                // On se met au début du fichier audio
                _musicPlayer.CurrentTime = 0d;
            }
		}

		public void LoadFromUrl(string url)
		{
            
            // Si un player existe on le supprime.
			StopAndDispose();

			// On stoque l'url de la musique courante
			_currentUrl = url;

            // On transforme l'url en NSURL
			NSUrl musicURL = NSUrl.FromString(url);

            // On charge le player en fonction de cette URL
			_musicPlayer = AVAudioPlayer.FromUrl(musicURL);
			_musicPlayer.Volume = _currentVolume;

			_musicPlayer.FinishedPlaying += _musicPlayer_FinishedPlaying;

			//Précharge la musique en mémoire
			_musicPlayer.PrepareToPlay();

            // Si le nombre de boucle est égale à -1 la musique est jouée en boucle
            _musicPlayer.NumberOfLoops = _currentNumbersOfLoops;
		}

        void _musicPlayer_FinishedPlaying(object sender, AVStatusEventArgs args)
        {
            Stop();

            if (FinishedPlaying != null)
            {
                FinishedPlaying();
            }
        }

        public void StopAndDispose()
		{
            
			if (_musicPlayer != null)
			{
                // On stoppe la musique courante
				_musicPlayer.Stop();

				//Puis on libère les ressources du lecteur audio
				_musicPlayer.Dispose();
                _musicPlayer = null;
			}
		}
	}
}