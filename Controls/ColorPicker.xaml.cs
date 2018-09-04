using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ghost.Controls
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : UserControl
    {
        public String RGBValue;
        public String WebValue;
        public Color ColorRectangleValue;
        private Color selectedPixelColor;
        public BitmapSource BitmapSourceControl;
        private PixelFormat pf = PixelFormats.Bgr32;
        private CroppedBitmap cb;

        public ColorPicker()
        {
            InitializeComponent();
        }

        public Color PickColor(FrameworkElement img)
        {
            double x = Mouse.GetPosition(img).X;
            x *= BitmapSourceControl.PixelWidth / img.ActualWidth;
            if ((int)x > BitmapSourceControl.PixelWidth - 1)
            {
                x = BitmapSourceControl.PixelWidth - 1;
            }
            else if (x < 0)
            {
                x = 0;
            }

            double y = Mouse.GetPosition(img).Y;
            y *= BitmapSourceControl.PixelHeight / img.ActualHeight;
            if ((int)y > BitmapSourceControl.PixelHeight - 1)
            {
                y = BitmapSourceControl.PixelHeight - 1;
            }
            else if (y < 0)
            {
                y = 0;
            }

            if (BitmapSourceControl != null)
            {
                try
                {
                    int stride = (BitmapSourceControl.PixelWidth * pf.BitsPerPixel + 7) / 8;
                    byte[] pixels = new byte[BitmapSourceControl.PixelHeight * stride];
                    cb = new CroppedBitmap(BitmapSourceControl, new Int32Rect((int)x, (int)y, 1, 1));

                    BitmapSourceControl.CopyPixels(new Int32Rect((int)x, (int)y, 1, 1), pixels, stride, 0);
                    return Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
                }
                catch
                {
                    return Color.FromArgb(255, 255, 255, 255);
                }
            }
            else
            {
                return Color.FromArgb(255, 255, 255, 255);
            }
        }

        public void SetColorInfo(MouseEventArgs e)
        {
            try
            {
                FrameworkElement img = (FrameworkElement)e.OriginalSource;

                selectedPixelColor = PickColor(img);
                RGBValue = String.Format("{0},{1},{2}", selectedPixelColor.R, selectedPixelColor.G, selectedPixelColor.B);
                WebValue = selectedPixelColor.ToString().Remove(0, 3);
                ColorRectangleValue = selectedPixelColor;
                UpdateTool();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void UpdateTool()
        {
            txt_rgbValues.Text = RGBValue;
            txt_webValues.Text = WebValue.ToUpper();
            ColorRectangle.Fill = new SolidColorBrush(Color.FromRgb(ColorRectangleValue.R, ColorRectangleValue.G, ColorRectangleValue.B));
        }
    }
}