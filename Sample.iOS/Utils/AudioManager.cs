using System;
using AVFoundation;
using Foundation;

namespace Sample.iOS.Utils
{
	public class AudioManager
	{
		private static AudioManager _audioManager;
		private AVAudioPlayer _musicPlayer;
		private const float _DEFAULT_VOLUME = 1.0f;
        private const int _DEFAULT_NUMBERS_OF_LOOPS = -1;

		private AudioManager()
		{
		}

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
					return _DEFAULT_VOLUME;
				}

				return _musicPlayer.Volume;
			}
			set
			{
				_musicPlayer.Volume = value;
			}
		}

		public nint NumberOfLoops
		{
			get
			{
                if(_musicPlayer == null)
                {
                    
                }
				return _musicPlayer.NumberOfLoops;
			}
			set
			{
				_musicPlayer.NumberOfLoops = value;
			}
		}

		public void Play()
		{
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
                _musicPlayer.CurrentTime = 0d;
            }
		}

		public void LoadFromUrl(string url)
		{
			DestroyOldPlayer();
			NSUrl musicURL = NSUrl.FromString(url);
			_musicPlayer = AVAudioPlayer.FromUrl(musicURL);
			_musicPlayer.Volume = _DEFAULT_VOLUME;
			_musicPlayer.FinishedPlaying += delegate
			{
				_musicPlayer.Dispose();
				_musicPlayer = null;
			};
			// on joue la musique en boucle
            _musicPlayer.NumberOfLoops = _DEFAULT_NUMBERS_OF_LOOPS;
		}

		public void LoadFromData(byte[] bytes)
		{
			DestroyOldPlayer();

			using (NSData data = NSData.FromArray(bytes))
			{
				// Si celui arrive à se charger on crée le player audio
				if (data != null)
				{
					_musicPlayer = AVAudioPlayer.FromData(data);
				}
				else
				{
					Console.WriteLine("Impossible de chargé le fichier audio");
				}
			}

			_musicPlayer.Volume = _DEFAULT_VOLUME;
			_musicPlayer.FinishedPlaying += delegate
			{
				_musicPlayer.Dispose();
				_musicPlayer = null;
			};
			_musicPlayer.NumberOfLoops = _DEFAULT_NUMBERS_OF_LOOPS;
		}

		private void DestroyOldPlayer()
		{
			if (_musicPlayer != null)
			{
				_musicPlayer?.Stop();
				_musicPlayer?.Dispose();
			}
		}
	}
}
