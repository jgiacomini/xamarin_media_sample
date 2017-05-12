using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Audio;
using Windows.Media.Capture;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Media.Render;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

namespace Sample.UWP
{
    public sealed partial class MainPage : Page
    {
        private AudioGraph _graph;
        AudioFileOutputNode _outputNode;

        public MainPage()
        {
            this.InitializeComponent();

            Uri uri = new Uri("https://www.xamarin.com/content/images/pages/branding/assets/xamarin-logo.png");
            MainImage.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(uri);
        }

        private async void Button_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            FileOpenPicker photoPicker = new FileOpenPicker();
            photoPicker.ViewMode = PickerViewMode.Thumbnail;
            photoPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            photoPicker.FileTypeFilter.Add(".jpg");
            photoPicker.FileTypeFilter.Add(".jpeg");
            photoPicker.FileTypeFilter.Add(".png");
            photoPicker.FileTypeFilter.Add(".bmp");

            StorageFile photoFile = await photoPicker.PickSingleFileAsync();
            if (photoFile == null)
            {
                return;
            }
        }

        private async void Start_Recording_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var microphone = await DeviceInformation.CreateFromIdAsync(MediaDevice.GetDefaultAudioCaptureId(AudioDeviceRole.Default));
            if (microphone != null)
            {

                var result = await AudioGraph.CreateAsync(
                 new AudioGraphSettings(AudioRenderCategory.Speech));

                if (result.Status == AudioGraphCreationStatus.Success)
                {
                    this._graph = result.Graph;

                    // 16K sampled, mono, 16-bit output
                    var outProfile = MediaEncodingProfile.CreateWav(AudioEncodingQuality.Low);
                    outProfile.Audio = AudioEncodingProperties.CreatePcm(16000, 1, 16);

                    // Création du fichier cible
                    var outputFile = await this.PickFileAsync();
                    if (outputFile == null)
                    {
                        // Opération annulée par l'utilisateur
                        return;
                    }

                    // Création du noeud d'enregistrement
                    var outputResult = await this._graph.CreateFileOutputNodeAsync(outputFile, outProfile);
                    if (outputResult.Status == AudioFileNodeCreationStatus.Success)
                    {
                        this._outputNode = outputResult.FileOutputNode;

                        // Création du profil d'enregistrement
                        //var inProfile = MediaEncodingProfile.CreateWav(AudioEncodingQuality.High);
                        var inProfile = this._graph.EncodingProperties;

                        // Création du noeud d'entrée
                        var inputResult = await this._graph.CreateDeviceInputNodeAsync(MediaCategory.Speech, inProfile.Audio, microphone);

                        if (inputResult.Status == AudioDeviceNodeCreationStatus.Success)
                        {
                            inputResult.DeviceInputNode.AddOutgoingConnection(this._outputNode);

                            this._graph.Start();
                        }
                    }
                }
            }
        }

        private async Task<StorageFile> PickFileAsync()
        {
            try
            {
                FileSavePicker fileSavePicker = new FileSavePicker();
                fileSavePicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
                fileSavePicker.SuggestedFileName = "sound";
                fileSavePicker.FileTypeChoices.Add("WAV files", new List<string>() { ".wav" });

                var outputFile = await fileSavePicker.PickSaveFileAsync();
                return outputFile;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        private async void Stop_Recording_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (this._graph != null)
            {
                this._graph.Stop();

                await this._outputNode.FinalizeAsync();

                // assuming that disposing the graph gets rid of the input/output nodes?
                this._graph.Dispose();

                this._graph = null;
            }
        }
    }
}
