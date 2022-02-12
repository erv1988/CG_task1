using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static System.Drawing.Bitmap MainBitmap;
        static Svg.SvgDocument SvgDoc;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BMPLoadImage(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            bool? ret = ofd.ShowDialog();
            if (ret.HasValue && ret.Value)
            {
                if (MainBitmap != null)
                {
                    MainBitmap.Dispose();
                }

                MainBitmap = System.Drawing.Image.FromFile(ofd.FileName) as System.Drawing.Bitmap;
                imageViewBmp.Source = MainBitmap.GetImageSource();
            }
        }

        private void BMPOnlyRed(object sender, RoutedEventArgs e)
        {
            selectChannel(2);
        }

        private void BMPOnlyGreen(object sender, RoutedEventArgs e)
        {
            selectChannel(1);
        }

        private void BMPOnlyBlue(object sender, RoutedEventArgs e)
        {
            selectChannel(0);
        }

        private void selectChannel(int id)
        {
            if (MainBitmap != null)
            {
                System.Drawing.Bitmap clone = MainBitmap.Clone() as System.Drawing.Bitmap;
                System.Drawing.Imaging.BitmapData dataSrc = MainBitmap.LockBits(new System.Drawing.Rectangle(0, 0, MainBitmap.Width, MainBitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, MainBitmap.PixelFormat);
                System.Drawing.Imaging.BitmapData dataDst = clone.LockBits(new System.Drawing.Rectangle(0, 0, clone.Width, clone.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, clone.PixelFormat);

                int bytes = Math.Abs(dataSrc.Stride) * dataSrc.Height;
                byte[] rgbValuesSrc = new byte[bytes];
                byte[] rgbValuesDst = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(dataSrc.Scan0, rgbValuesSrc, 0, bytes);

                for (int y = 0; y < dataSrc.Height; ++y)
                {
                    int idx = y * dataSrc.Stride;
                    for (int x = 0; x < dataSrc.Width; ++x, idx += 3)
                    {
                        for (int i = 0; i < 3; ++i)
                        {
                            rgbValuesDst[idx + i] = i == id ? rgbValuesSrc[idx + i] : (byte)0;
                        }
                    }
                }

                System.Runtime.InteropServices.Marshal.Copy(rgbValuesDst, 0, dataDst.Scan0, bytes);

                MainBitmap.UnlockBits(dataSrc);
                clone.UnlockBits(dataDst);

                imageViewBmp.Source = clone.GetImageSource();
            }
        }

        private void BMPClear10(object sender, RoutedEventArgs e)
        {
            if (MainBitmap != null)
            {
                System.Drawing.Bitmap clone = MainBitmap.Clone() as System.Drawing.Bitmap;
                System.Drawing.Imaging.BitmapData dataSrc = MainBitmap.LockBits(new System.Drawing.Rectangle(0, 0, MainBitmap.Width, MainBitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, MainBitmap.PixelFormat);
                System.Drawing.Imaging.BitmapData dataDst = clone.LockBits(new System.Drawing.Rectangle(0, 0, clone.Width, clone.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, clone.PixelFormat);

                int bytes = Math.Abs(dataSrc.Stride) * dataSrc.Height;
                byte[] rgbValuesSrc = new byte[bytes];
                byte[] rgbValuesDst = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(dataSrc.Scan0, rgbValuesSrc, 0, bytes);

                for (int y = 0; y < dataSrc.Height; ++y)
                {
                    int idx = (dataSrc.Height - y - 1) * dataSrc.Stride;
                    for (int x = 0; x < dataSrc.Width; ++x, idx += 3)
                    {
                        for (int i = 0; i < 3; ++i)
                        {
                            rgbValuesDst[idx + i] = y >= 10 ? rgbValuesSrc[idx + i] : (byte)0;
                        }
                    }
                }

                System.Runtime.InteropServices.Marshal.Copy(rgbValuesDst, 0, dataDst.Scan0, bytes);

                MainBitmap.UnlockBits(dataSrc);
                clone.UnlockBits(dataDst);

                imageViewBmp.Source = clone.GetImageSource();
            }
        }

        private void BMPSave(object sender, RoutedEventArgs e)
        {
            BitmapImage image = imageViewBmp.Source as BitmapImage;
            MemoryStream dst = new MemoryStream();
            MemoryStream src = image.StreamSource as MemoryStream;
            long srcPos = src.Position;
            src.Position = 0;
            src.CopyTo(dst);
            src.Position = srcPos;
            dst.Position = 0;
            System.Drawing.Image bitmap = System.Drawing.Image.FromStream(dst);
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            bool? ret = sfd.ShowDialog();
            if (ret.HasValue && ret.Value)
            {
                bitmap.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
            }
            dst.Dispose();
            bitmap.Dispose();
        }

        private void SVGLoadImage(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            bool? ret = ofd.ShowDialog();
            if (ret.HasValue && ret.Value)
            {
                updateSvg(Svg.SvgDocument.Open(ofd.FileName));
            }
        }

        private void updateSvg(Svg.SvgDocument svgdoc)
        {
            SvgDoc = svgdoc;
            var bitmap = SvgDoc.Draw();
            imageViewSvg.Source = bitmap.GetImageSource();
            MemoryStream ms = new MemoryStream();
            SvgDoc.Write(ms);

            imageContentSvg.Text = File.ReadAllText(SvgDoc.BaseUri.OriginalString);
            ms.Dispose();
        }

        private void SVGLoadImageFromText(object sender, RoutedEventArgs e)
        {
            try
            {
                string tempFile = Path.GetTempFileName();
                File.WriteAllText(tempFile, imageContentSvg.Text, System.Text.Encoding.UTF8);
                updateSvg(Svg.SvgDocument.Open(tempFile));
                File.Delete(tempFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

    }
}
