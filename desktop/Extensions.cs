using System.IO;
using System.Windows.Media.Imaging;

namespace desktop
{
    public static class Extensions
    {

        public static System.Windows.Media.ImageSource GetImageSource(this System.Drawing.Image bitmap)
        {
            if (bitmap == null)
                return null;
            MemoryStream memStream = new MemoryStream();
            bitmap.Save(memStream, System.Drawing.Imaging.ImageFormat.Png);

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnDemand;
            image.StreamSource = memStream;
            image.EndInit();

            return image;
        }



    }
}
