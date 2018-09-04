using System;
using System.Windows.Media.Imaging;

namespace Ghost.Controls
{
    public class Photo
    {
        public Photo(string path)
        {
            _path = path;
            _source = new Uri(path);
            BitmapDecoder uriBitmap;

            if (_path.IndexOf(".png") != -1)
            {
                uriBitmap = PngBitmapDecoder.Create(_source, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnDemand);
            }
            else if (_path.IndexOf(".gif") != -1)
            {
                uriBitmap = GifBitmapDecoder.Create(_source, BitmapCreateOptions.None, BitmapCacheOption.OnDemand);
            }
            else if (_path.IndexOf(".jpg") != -1)
            {
                uriBitmap = JpegBitmapDecoder.Create(_source, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnDemand);
            }
            else
            {
                uriBitmap = BitmapDecoder.Create(_source, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnDemand);
            }

            _image = BitmapFrame.Create(uriBitmap.Frames[0]);
        }

        public override string ToString()
        {
            return _source.ToString();
        }

        private string _path;
        private Uri _source;
        public string Source { get { return _path; } }
        private BitmapFrame _image;
        public BitmapFrame Image { get { return _image; } set { _image = value; } }
    }
}