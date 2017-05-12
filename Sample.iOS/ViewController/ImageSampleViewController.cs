using System;
using System.Diagnostics;
using System.IO;
using CoreGraphics;
using Foundation;
using UIKit;
namespace Sample.iOS
{
	public class ImageSampleViewController : UIViewController
	{
		private UIImagePickerController _imagePicker;
		private UIImageView _imageViewFromImagePicker;

		public ImageSampleViewController()
		{
			Title = "Image sample";
			View.BackgroundColor = UIColor.White;
			this.EdgesForExtendedLayout = UIRectEdge.None;
		}


		UIImage LoadImageFromBundle()
		{
			var width = 100;
			var height = 100;

			var imageViewFromFile = new UIImageView();
			imageViewFromFile.Frame = new CGRect(View.Frame.Width / 2 - width / 2, View.Frame.Height / 2 - height / 2, width, height);
			imageViewFromFile.Image = UIImage.FromBundle("MonImage.jpg");

			View.AddSubview(imageViewFromFile);

			return imageViewFromFile.Image;
		}

		void LoadFromBytesArray()
		{
			var bytes = File.ReadAllBytes("MonImage.jpg");

			// Création de l'imageView
			var imageViewFromBytesArray = new UIImageView();
			UIImage image = null;

			// Largeur de l'image
			var width = 50;
			// Hauteur de l'image
			var height = 50;

			// On place l'image au centre de la vue princiaple
            imageViewFromBytesArray.Frame = new CGRect(View.Frame.Width / 2 - width / 2, (View.Frame.Height + 4 * height) / 2 - height / 2, width, height);

			// Chargement d'un NSData à partir du tableau d'octet
			using (NSData data = NSData.FromArray(bytes))
			{
				// Si celui arrive à se charger on crée l'image
				if (data != null)
				{
					image = UIImage.LoadFromData(data);
				}
			}

			//Association l'image à l'ImageView
			imageViewFromBytesArray.Image = image;
			View.AddSubview(imageViewFromBytesArray);
		}

		void LoadFromURL()
		{
			// Largeur de l'image
			var width = 50;
			// Hauteur de l'image
			var height = 50;

			var imageViewFromURL = new UIImageView();
			UIImage image = null;
            imageViewFromURL.Frame = new CGRect(View.Frame.Width / 2 - width / 2, 100, width, height);

			// Création de la chaîne de caractère contenant l'adresse de l'image
			string uri = "http://www.elfo.net/wp-content/uploads/2015/06/BoxImgBlog_xamarin-6701.jpg";

			//Conversion en NSURL
			using (var url = new NSUrl(uri))
			{
				using (NSData data = NSData.FromUrl(url))
				{
					if (data != null)
					{
						image = UIImage.LoadFromData(data);
					}
                    else
                    {
                        Console.WriteLine(string.Format("Impossible de chargé l'image {0}", url));
                    }
				}
			}
			//Association l'image à l'ImageView
			imageViewFromURL.Image = image;
			imageViewFromURL.UserInteractionEnabled = true;

			View.AddSubview(imageViewFromURL);
		}

		#region ImagePicker
		void BuildImagePickerAndButtons()
		{ 
			// Exemple d'utilisation de l'image Image picker
			_imageViewFromImagePicker = new UIImageView();
			_imageViewFromImagePicker.Frame = new CGRect(0, 80, View.Frame.Width, View.Frame.Height - 80);

			UIButton imagePickerButton = UIButton.FromType(UIButtonType.System);
			imagePickerButton.BackgroundColor = UIColor.Red;
			imagePickerButton.SetTitle("Picker", UIControlState.Normal);
			imagePickerButton.TouchUpInside += ImagePickerButton_TouchUpInside;
			imagePickerButton.Frame = new CGRect(10, 10, View.Bounds.Width / 2 - 20, 40);

			UIButton clearImagePickerButton = UIButton.FromType(UIButtonType.System);
			clearImagePickerButton.BackgroundColor = UIColor.Red;
			clearImagePickerButton.SetTitle("Supprimer", UIControlState.Normal);
			clearImagePickerButton.TouchUpInside += ClearImagePickerButton_TouchUpInside; ;
			clearImagePickerButton.Frame = new CGRect(View.Bounds.Width / 2 + 10, 10, View.Bounds.Width / 2 - 20, 40);

			_imagePicker = new UIImagePickerController();
			// Définit la type de source
			_imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
			// Définit le type de media disponible
			_imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);
			_imagePicker.FinishedPickingMedia += _imagePicker_FinishedPickingMedia;

			_imagePicker.Canceled += _imagePicker_Canceled;

			View.AddSubview(_imageViewFromImagePicker);
			View.AddSubviews(imagePickerButton, clearImagePickerButton);
		}

		void ImagePickerButton_TouchUpInside(object sender, EventArgs e)
		{
			//PresentModalViewController is depreciated in iOS6 so we use PresentViewController
			this.PresentViewController(_imagePicker, true, null);
		}

		void ClearImagePickerButton_TouchUpInside(object sender, EventArgs e)
		{
			_imageViewFromImagePicker.Image = null;
		}

		void _imagePicker_FinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs e)
		{
			// Vérifie que l'utilisateur a bien sélectionné un image
			if (e.Info[UIImagePickerController.MediaType].ToString() == "public.image")
			{
				UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;
				if (originalImage != null)
				{
					_imageViewFromImagePicker.Image = originalImage;
				}

			}
			else
			{
				Console.WriteLine("Image non selectionnée");
			}

			// Ferme le sélecteur d'image
			_imagePicker.DismissModalViewController(true);
		}

		void _imagePicker_Canceled(object sender, EventArgs e)
		{
			// Ferme le sélecteur d'image
			_imagePicker.DismissModalViewController(true);
			Console.WriteLine("Sélection de l'image annulée");
		}
		#endregion

		#region SaveImage
		void SavePNGImage(UIImage image)
		{
			using (NSData pngData = image.AsPNG())
			{
				var documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				string fileName = System.IO.Path.Combine(documentsDirectory, "image.png");

				NSError err = null;
				if (pngData.Save(fileName, false, out err))
				{
					Console.WriteLine($"Image sauvegardée {fileName}");
				}
				else
				{
					Console.WriteLine($"Image non sauvegardée car :{err.LocalizedDescription}");
				}
			}
		}

		void SaveJPGImage(UIImage image)
		{
			using (NSData data = image.AsJPEG())
			{
				var documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				string fileName = System.IO.Path.Combine(documentsDirectory, "image.jpg");

				NSError err = null;
				if (data.Save(fileName, false, out err))
				{
					Console.WriteLine($"Image sauvegardée {fileName}");
				}
				else
				{
					Console.WriteLine($"Image non sauvegardée car :{err.LocalizedDescription}");
				}
			}
		}

		void SavePhotoToUserAlbum(UIImage imageToSave)
		{ 
			imageToSave.SaveToPhotosAlbum((image, error) =>
			{
				if (error != null)
				{
					Console.WriteLine("Impossible de sauvergardé l'image dans l'album photo de l'utilisateur:" + error);
				}
				else
				{
					Console.WriteLine("Image sauvegardé dans l'album photo de l'utilisateur");
				}
			});
		}

		#endregion
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var image = LoadImageFromBundle();

            LoadFromBytesArray();
			LoadFromURL();

			BuildImagePickerAndButtons();


			//Sauvegarde de en PNG
			SavePNGImage(image);
		
			//Sauvegarde de en JPG
			SaveJPGImage(image);

			//Sauvegarde de dans l'album photo de l'utilisateur
			SavePhotoToUserAlbum(image);
		
		}
	}
}
