using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Widget;
using Square.Picasso;
using System.IO;

namespace Sample.Droid
{
    [Activity(Label = "Sample.Droid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private ImageView _myImage;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            _myImage = FindViewById<ImageView>(Resource.Id.myImage);
            if (_myImage != null)
            {
                // Chargement à partir des ressources
                Picasso.With(this).Load(Resource.Drawable.Icon).Into(_myImage);

                // Chargement à partir d'un tableau d'octets
                var bytes = new byte[512];

                Bitmap bitmap = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
                _myImage.SetImageBitmap(bitmap);

                // Chargement à partr d'une URL
                Picasso.With(this).Load("https://www.android.com/static/2016/img/logo-android-green_1x.png").Into(_myImage);

                // Chargement d'une image à partir de la galerie
                var getImageIntent = new Intent();
                getImageIntent.SetType("image/*");
                getImageIntent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(getImageIntent, "Sélectionnez une photo"), 0);

                // Enregistrement d'une image
                BitmapDrawable bitmapDrawable = (BitmapDrawable)_myImage.Drawable;
                Bitmap anotherBitmap = bitmapDrawable.Bitmap;

                string filepath = System.IO.Path.Combine(Environment.ExternalStorageDirectory.AbsolutePath, "myAwesomeImage.png");
                using (var fileStream = new FileStream(filepath, FileMode.OpenOrCreate))
                {
                    anotherBitmap.Compress(Bitmap.CompressFormat.Png, 90, fileStream);
                }
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                Picasso.With(this).Load(data.Data).Into(_myImage);
            }
        }
    }
}