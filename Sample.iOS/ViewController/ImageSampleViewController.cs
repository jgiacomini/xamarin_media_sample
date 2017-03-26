using System;
using CoreGraphics;
using Foundation;
using UIKit;
namespace Sample.iOS
{
	public class ImageSampleViewController : UIViewController
	{
		private UIImagePickerController _imagePicker;

		public ImageSampleViewController()
		{
			Title = "Image sample";
			View.BackgroundColor = UIColor.White;
			this.EdgesForExtendedLayout = UIRectEdge.None;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			UIImageView imageView = new UIImageView(View.Frame);
			imageView.Image = UIImage.FromBundle("MonImage.png");

			UIImageView imageViewUrl = null;
			UIImage imageURL = null;
			// Création de la chaîne de caractère contenant l'adresse de l'image
			string uri = "http://xamarin.com/monkey.jpg";

			//Conversion en NSURL
			using (var url = new NSUrl(uri))
			{
				using (NSData data = NSData.FromUrl(url))
				{
					if (data != null)
					{
						imageURL = UIImage.LoadFromData(data);
					}
				}
			}


			if (imageURL != null)
			{
				imageViewUrl = new UIImageView(imageURL);
			}

			UIButton imagePickerButton = UIButton.FromType(UIButtonType.System);
			imagePickerButton.SetTitle("Picker", UIControlState.Normal);
			imagePickerButton.TouchUpInside += ImagePickerButton_TouchUpInside;;
			imagePickerButton.Frame = new CGRect(10, 10, View.Bounds.Width, 40);
			View.AddSubview(imagePickerButton);


			_imagePicker = new UIImagePickerController();
			// Définit la type de source
			_imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
			// Définit le type de media disponible
			_imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);
			_imagePicker.FinishedPickingMedia += _imagePicker_FinishedPickingMedia;
			_imagePicker.Canceled += _imagePicker_Canceled;

		}

		void ImagePickerButton_TouchUpInside(object sender, EventArgs e)
		{
			//PresentModalViewController is depreciated in iOS6 so we use PresentViewController
			this.PresentViewController(_imagePicker, true, null);
		}

		void _imagePicker_FinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs e)
		{
			// Vérifie que l'utilisateur a bien sélectionné un image
			if (e.Info[UIImagePickerController.MediaType].ToString() == "public.image")
			{
				UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;
				if (originalImage != null)
				{
					Console.WriteLine("Récupération de l'image originale");
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
	}
}
